using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ispProject.Models;
using ispProject.Security;
using System.Web.Security;
using NLog;

namespace ispProject.Controllers
{
    public class usersController : Controller
    {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: users
        [Authorize]
        public ActionResult Index()
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                var users = db.users.Include(u => u.hospital).Include(u => u.role);
                return View(users.ToList());
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        public ActionResult adminPanel()
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                return View();
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }


        // GET: users/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            //if (Request.Cookies["role"] != null)
            //{
            //    HttpCookie aCookie = Request.Cookies["role"];
            //    ViewBag.role = Server.HtmlEncode(aCookie.Value);
            //}

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            //Get Signed in account
            user account = db.users.Find(UserAccount.GetUserID());
            //If it's their account and they are lower-tier, they can't edit their hospital or role.
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                return View(user);
            }
            else
            {
                if (account.userId == user.userId)
                {
                    return View(user);
                }
                else
                {
                    return RedirectToAction("tempError", "Home");
                }
            }
        }

        // GET: users/Create
        public ActionResult Create()
        {
            ViewBag.hospitalId = new SelectList(db.hospitals, "hospital_id", "name");
            ViewBag.roleId = new SelectList(db.roles, "roleId", "title");
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,firstName,lastName,hospitalId,roleId,dateCreated,dateUpdated,editedBy")] user user, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string email = form["email"].ToString();
                if (db.userAccounts.Where(accounts => accounts.userName == email).Count() == 0)
                {

                    //TODO make these two ID assignments db queries to find ID where hospital = "WCTC" and role = account with lowest permissions
                    user.hospitalId = 1;
                    user.roleId = 5;
                    user.dateCreated = DateTime.Today;
                    user.dateUpdated = DateTime.Today;
                    user.editedBy = 1;
                    db.users.Add(user);
                    db.SaveChanges();

                    List<user> userList = db.users.Where(u => u.firstName == user.firstName && u.lastName == user.lastName && u.dateCreated == user.dateCreated).ToList();
                    userAccount uA = new userAccount();
                    uA.userId = userList[0].userId;
                    uA.userName = email;
                    uA.userGuid = System.Guid.NewGuid();
                    uA.passwordHash = UserAccount.HashSHA1(form["password"] + uA.userGuid);
                    uA.dateCreated = DateTime.Now;
                    uA.dateUpdated = DateTime.Now;
                    uA.editedBy = 1;
                    db.userAccounts.Add(uA);
                    db.SaveChanges();
                    logger.Info("User " + uA.userName + " created");

                    //Get Signed in account
                    try
                    {
                        user account = db.users.Find(UserAccount.GetUserID());
                        //If it's their account and they are lower-tier, they can't edit their hospital or role.
                        if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        TempData["success"] = "Account " + uA.userName + " created! You may now sign in with limited access. Please see your instructor to have your account activated.";
                        return RedirectToAction("Index", "Home");

                    }

                }
                TempData["error"] = "Email already registered. Please sign in.";
                logger.Info("User tried to create an account with already registered email: " + email);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.hospitalId = new SelectList(db.hospitals, "hospital_id", "name", user.hospitalId);
            ViewBag.roleId = new SelectList(db.roles, "roleId", "title", user.roleId);
            return View(user);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(FormCollection form, string ReturnUrl)
        {
            using (HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1())
            {
                var userEmail = form["email"];
                List<userAccount> accountList = db.userAccounts.Where(u => u.userName == userEmail).ToList();

                //Verify list returned an account.
                if (accountList.Any())
                {
                    userAccount uA = accountList[0];

                    string str = UserAccount.HashSHA1(form["password"] + uA.userGuid);

                    if (str == uA.passwordHash)
                    {
                        user user = db.users.Find(uA.userId);
                        FormsAuthentication.SetAuthCookie(uA.userId.ToString(), false);
                        HttpCookie userRoleCookie = new HttpCookie("role");
                        userRoleCookie.Value = user.role.title;
                        Response.Cookies.Add(userRoleCookie);
                        HttpCookie userIdCookie = new HttpCookie("userId");
                        userIdCookie.Value = Convert.ToString(user.userId);
                        Response.Cookies.Add(userIdCookie);
                        if (ReturnUrl != null)
                        {
                            return Redirect(ReturnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    TempData["error"] = "Sorry, invalid user credentials. Please try again";
                    ModelState.AddModelError("Password", "Incorrect password");
                }
                else
                {
                    TempData["error"] = "Sorry, an account with that email was not found.";
                    logger.Info("User tried to login with an invalid email: " + userEmail);
                }

            }
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // GET: users/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            //Get Signed in account
            user account = db.users.Find(UserAccount.GetUserID());
            //If it's their account and they are lower-tier, they can't edit their hospital or role.
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                //If the account is priveledged or the owner, they can edit the account.
                ViewBag.hospitalId = new SelectList(db.hospitals, "hospitalId", "name", user.hospitalId);
                ViewBag.roleId = new SelectList(db.roles, "roleId", "title", user.roleId);
                ViewBag.notHidden = true;
                //ViewBag.hospitalId = new SelectList(db.hospitals.Where(h => h.hospitalId == user.hospitalId), "hospitalId", "name", user.hospitalId);
                //ViewBag.roleId = new SelectList(db.roles.Where(r => r.roleId == user.roleId), "roleId", "title", user.roleId);
                return View(user);
            }
            else
            {
                if (account.userId == user.userId)
                {
                    ViewBag.hospitalId = new SelectList(db.hospitals.Where(h => h.hospital_id == user.hospitalId), "hospitalId", "name", user.hospitalId);
                    ViewBag.roleId = new SelectList(db.roles.Where(r => r.roleId == user.roleId), "roleId", "title", user.roleId);
                    ViewBag.notHidden = null;
                    return View(user);
                }
                else
                {
                    return RedirectToAction("tempError", "Home");
                }
            }
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,firstName,lastName,hospitalId,roleId,dateCreated,dateUpdated,editedBy")] user user, FormCollection form)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (ModelState.IsValid)
                {
                    user u = db.users.Find(user.userId);
                    userAccount uA = db.userAccounts.Where(p => p.userId == user.userId).First();
                    uA.userName = form["email"];
                    var password = form["password"];
                    if (form["password"] != "")
                    {
                        uA.passwordHash = UserAccount.HashSHA1(form["password"] + uA.userGuid);
                    }
                    db.Entry(u).CurrentValues.SetValues(user);
                    db.Entry(uA).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " edited details of " + user.userId + "(" + user.firstName + " " + user.lastName + ")");
                    return RedirectToAction("Index");
                }
                ViewBag.hospitalId = new SelectList(db.hospitals, "hospitalId", "name", user.hospitalId);
                ViewBag.roleId = new SelectList(db.roles, "roleId", "title", user.roleId);
                return View(user);
            }
            else if (account.userId == user.userId)
            {
                if (ModelState.IsValid)
                {
                    user u = db.users.Find(user.userId);
                    userAccount uA = db.userAccounts.Where(p => p.userId == user.userId).First();
                    uA.userName = form["email"];
                    if (form["password"] != null || form["password"] != "")
                    {
                        uA.passwordHash = UserAccount.HashSHA1(form["password"] + uA.userGuid);
                    }
                    db.Entry(u).CurrentValues.SetValues(user);
                    db.Entry(uA).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " edited details of " + user.userId + "(" + user.firstName + " " + user.lastName + ")");
                    return RedirectToAction("Index");
                }
                ViewBag.hospitalId = new SelectList(db.hospitals, "hospitalId", "name", user.hospitalId);
                ViewBag.roleId = new SelectList(db.roles, "roleId", "title", user.roleId);
                return View(user);
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " attempted to edit the details of " + user.userId);
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: users/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                user user = db.users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                user user = db.users.Find(id);
                userAccount uA = db.userAccounts.Where(u => u.userId == user.userId).First();
                db.users.Remove(user);
                db.userAccounts.Remove(uA);
                db.SaveChanges();
                //logger.Info("User " + account.firstName + " " + account.lastName + " deleted user " + id);
                return RedirectToAction("Index");
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " attempted to delete user " + id);
                return RedirectToAction("tempError", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}