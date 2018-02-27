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
using NLog;

namespace ispProject.Controllers
{
    [Authorize]
    public class hospitalsController : Controller
    {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: hospitals
        public ActionResult Index()
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                return View(db.hospitals.ToList());
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: hospitals/Details/5
        public ActionResult Details(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title != "Data Entry")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                hospital hospital = db.hospitals.Find(id);
                if (hospital == null)
                {
                    return HttpNotFound();
                }
                return View(hospital);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: hospitals/Create
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

        // POST: hospitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "hospitalId,name,dateCreated,dateUpdated,editedBy")] hospital hospital)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (ModelState.IsValid)
                {
                    hospital.date_added = DateTime.Now;
                    hospital.date_edited = DateTime.Now;
                    hospital.edited_by = 1;
                    db.hospitals.Add(hospital);
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " created hospital: " + hospital.name);
                    return RedirectToAction("Index");
                }

                return View(hospital);
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to create hospital: " + hospital.name);
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: hospitals/Edit/5
        public ActionResult Edit(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                hospital hospital = db.hospitals.Find(id);
                if (hospital == null)
                {
                    return HttpNotFound();
                }
                return View(hospital);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: hospitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "hospitalId,name,dateCreated,dateUpdated,editedBy")] hospital hospital)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(hospital).State = EntityState.Modified;
                    db.SaveChanges();
                    logger.Info("User " + account.firstName + " " + account.lastName + " edited hospital: " + hospital.name);
                    return RedirectToAction("Index");
                }
                return View(hospital);
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to edit hospital: " + hospital.name);
                return RedirectToAction("tempError", "Home");
            }
        }

        // GET: hospitals/Delete/5
        public ActionResult Delete(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                hospital hospital = db.hospitals.Find(id);
                if (hospital == null)
                {
                    return HttpNotFound();
                }
                return View(hospital);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: hospitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hospital hospital = db.hospitals.Find(id);
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Database Adminstrator" || account.role.title == "Instructor")
            {
                db.hospitals.Remove(hospital);
                db.SaveChanges();
                logger.Info("User " + account.firstName + " " + account.lastName + " deleted hospital: " + hospital.name);
                return RedirectToAction("Index");
            }
            else
            {
                logger.Info("User " + account.firstName + " " + account.lastName + " tried to delete hospital: " + hospital.name);
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
