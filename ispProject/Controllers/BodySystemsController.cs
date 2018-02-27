using ispProject.Models;
using ispProject.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ispProject.Classes;
using System.Data.Entity;

namespace ispProject.Controllers {
    public class BodySystemsController : Controller {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();

        // GET: BodySystems
        public ActionResult Index(int? pcaID) {
            if (pcaID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get one of each body system type with a matching pcaID that has the most recent date
            IQueryable<nursing_care_system_assessment> ncsas = db.nursing_care_system_assessment
                .Where(c => c.pca_id == pcaID && c.care_system_assessment_type_id != 1)
                .OrderBy(d => d.care_system_assessment_type_id)
                .GroupBy(e => e.care_system_assessment_type_id)
                .Select(f => f.OrderByDescending(g => g.date_care_system_added).FirstOrDefault());

            ViewBag.EncounterID = db.nursing_pca_record.Find(pcaID).encounter_id;
            ViewBag.pcaID = pcaID;

            return View(ncsas.ToList());
        }

        // GET: BodySystems/Details
        public ActionResult Details(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the body system with matching typeID and pcaID and the most recent date
            var bodySystem = from c in db.nursing_care_system_assessment
                             where c.care_system_assessment_type_id == typeID && c.pca_id == pcaID
                             group c by c.care_system_assessment_type_id into g
                             select g.OrderByDescending(t => t.date_care_system_added).FirstOrDefault();

            ViewBag.CareSystemTypeName = db.nursing_care_system_assessment_type.FirstOrDefault(t => t.care_system_assessment_type_id == typeID).care_system_assessment_type_name;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;
            ViewBag.EncounterID = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == pcaID).encounter_id;

            if (bodySystem.FirstOrDefault() == null) {
                var emptyCareSystem = new nursing_care_system_assessment();
                emptyCareSystem.pca_id = (int)pcaID;
                return View(emptyCareSystem);
            }
            return View(bodySystem.FirstOrDefault());
        }

        // GET: BodySystems/Edit
        public ActionResult Edit(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the body system with matching typeID and pcaID and the most recent date
            var bodySystemQ = from c in db.nursing_care_system_assessment
                             where c.care_system_assessment_type_id == typeID && c.pca_id == pcaID
                             group c by c.care_system_assessment_type_id into g
                             select g.OrderByDescending(t => t.date_care_system_added).FirstOrDefault();

            ViewBag.CareSystemTypeName = db.nursing_care_system_assessment_type.FirstOrDefault(t => t.care_system_assessment_type_id == typeID).care_system_assessment_type_name;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;

            if (bodySystemQ.FirstOrDefault() == null) {
                var emptyCareSystem = new nursing_care_system_assessment();
                emptyCareSystem.pca_id = (int)pcaID;
                emptyCareSystem.care_system_assessment_type_id = (int)typeID;
                return View(emptyCareSystem);
            }
            return View(bodySystemQ.FirstOrDefault());
        }

        // POST: BodySystems/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "wdl_ex, care_system_comment, date_care_system_added")] FormCollection form) {
            int ncsaID = int.Parse(form["care_system_assessment_id"]);
            nursing_care_system_assessment ncsa = db.nursing_care_system_assessment.FirstOrDefault(m => m.care_system_assessment_id == ncsaID);
            nursing_pca_record pca = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == ncsa.pca_id);

            if (form["formButton"] == "Exit") {
                return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
            }

            if (ModelState.IsValid) {
                nursing_care_system_assessment_history ncsahistory = new nursing_care_system_assessment_history();
                ncsahistory.care_system_assessment_id = ncsa.care_system_assessment_id;
                ncsahistory.care_system_assessment_type_id = ncsa.care_system_assessment_type_id;
                ncsahistory.pca_id = ncsa.pca_id;
                ncsahistory.date_pca_record_added = pca.date_vitals_added;
                ncsahistory.wdl_ex = ncsa.wdl_ex;
                ncsahistory.care_system_comment = ncsa.care_system_comment;
                ncsahistory.date_care_system_added = ncsa.date_care_system_added;
                ncsahistory.date_care_system_modified = DateTime.Now;

                ncsa.wdl_ex = form["wdlRadios"] == "wdlEx" ? true : false;
                ncsa.care_system_comment = form["wdlExceptionInfo"];
                
                db.SaveChanges();

                switch (form["formButton"]) {
                    case "SaveList":
                        return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
                    case "SaveContinue":
                        int typeID = (int)ncsa.care_system_assessment_type_id + 1;
                        if (typeID <= db.nursing_care_system_assessment_type.Count()) {
                            return RedirectToAction("CreateOrEdit", "BodySystems", new { pcaID = ncsa.pca_id,  typeID = typeID});
                        }
                        else {
                            return RedirectToAction("CreateOrEdit", "Comments", new { pcaID = ncsa.pca_id, typeID = 2 });
                        }

                    default:
                        return View(ncsa);
                }
            }
            return View(ncsa);
        }

        // GET: BodySystems/Create
        public ActionResult Create(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.pcaID = pcaID;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;

            nursing_care_system_assessment_type ncsat = db.nursing_care_system_assessment_type.FirstOrDefault(m => m.care_system_assessment_type_id == typeID);
            return View(ncsat);
        }

        // POST: BodySystem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "care_system_assessment_id, pca_id, wdl_ex, care_system_comment, date_care_system_added")] nursing_care_system_assessment ncsa, FormCollection form) {
            int pcaID = int.Parse(form["pcaID"]);
            nursing_pca_record pca = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == pcaID);

            if (form["formButton"] == "Exit") {
                return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
            }

            if (ModelState.IsValid) {
                ncsa.pca_id = pca.pca_id;
                ncsa.care_system_assessment_type_id = int.Parse(form["care_system_assessment_type_id"]);
                ncsa.wdl_ex = form["wdlRadios"] == "wdlEx" ? true : false;
                ncsa.care_system_comment = form["wdlExceptionInfo"];
                ncsa.date_care_system_added = DateTime.Now;
                db.nursing_care_system_assessment.Add(ncsa);
                db.SaveChanges();

                switch (form["formButton"]) {
                    case "SaveList":
                        return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
                    case "SaveContinue":
                        int typeID = (int)ncsa.care_system_assessment_type_id + 1;
                        if (typeID <= db.nursing_care_system_assessment_type.Count()) {
                            return RedirectToAction("CreateOrEdit", "BodySystems", new { pcaID = ncsa.pca_id, typeID = typeID });
                        }
                        else {
                            return RedirectToAction("Create", "Comments", new { pcaID = ncsa.pca_id, typeID = typeID });
                        }

                    default:
                        return View(ncsa);
                }
            }

            ViewBag.pcaID = pcaID;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            return View(db.nursing_care_system_assessment_type.Find(ncsa.care_system_assessment_type_id));
        }

        // GET: BodySystems/CreateOrEdit
        public ActionResult CreateOrEdit(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the body system with matching typeID and pcaID
            var bodySystemQ = from c in db.nursing_care_system_assessment
                             where c.care_system_assessment_type_id == typeID && c.pca_id == pcaID
                             group c by c.care_system_assessment_type_id into g
                             select g.FirstOrDefault();

            if (bodySystemQ.FirstOrDefault() == null) {
                return RedirectToAction("Create", "BodySystems", new { pcaID = pcaID, typeID = typeID });
            }
            else {
                return RedirectToAction("Edit", "BodySystems", new { pcaID = pcaID, typeID = typeID });
            }
        }
    }
}