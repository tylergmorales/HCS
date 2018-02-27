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
    [Authorize]
    public class rolesController : Controller
    {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: roles
        public ActionResult Index()
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                return View(db.roles.ToList());
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: roles/Details/5
        public ActionResult Details(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                role role = db.roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: roles/Create
        public ActionResult Create()
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

        // POST: roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "roleId,title,dateCreated,dateUpdated,editedBy")] role role)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (ModelState.IsValid)
                {
                    role.dateCreated = DateTime.Now;
                    role.dateUpdated = DateTime.Now;
                    role.editedBy = 1;
                    db.roles.Add(role);
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " created role: " + role.title);
                    return RedirectToAction("Index");
                }

                return View(role);
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to created role: " + role.title);
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: roles/Edit/5
        public ActionResult Edit(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                role role = db.roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "roleId,title,dateCreated,dateUpdated,editedBy")] role role)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(role).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " edited role: " + role.title);
                    return RedirectToAction("Index");
                }
                return View(role);
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to edit role: " + role.title);
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: roles/Delete/5
        public ActionResult Delete(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                role role = db.roles.Find(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            role role = db.roles.Find(id);
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                db.roles.Remove(role);
                db.SaveChanges();
                logger.Info("User " + account.firstName + " " + account.lastName + " deleted role: " + role.title);
                return RedirectToAction("Index");
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to delete role #: " + role.title);
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
