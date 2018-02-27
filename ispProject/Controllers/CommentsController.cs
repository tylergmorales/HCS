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
    public class CommentsController : Controller {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();

        // GET: Comments/Details
        public ActionResult Details(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the comment with matching typeID and pcaID and the most recent date
            var pcaComment = from c in db.nursing_pca_comment
                             where c.pca_comment_type_id == typeID && c.pca_id == pcaID
                             group c by c.pca_comment_type_id into g
                             select g.OrderByDescending(t => t.date_comment_added).FirstOrDefault();

            ViewBag.CommentTypeName = db.nursing_pca_comment_type.FirstOrDefault(t => t.pca_comment_type_id == typeID).pca_comment_type_name;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;
            ViewBag.EncounterID = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == pcaID).encounter_id;

            if (pcaComment.FirstOrDefault() == null) {
                var emptyComment = new nursing_pca_comment();
                emptyComment.pca_id = (int)pcaID;
                return View(emptyComment);
            }
            return View(pcaComment.FirstOrDefault());
        }

        // GET: Comments/Edit
        public ActionResult Edit(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the comment with matching typeID and pcaID and the most recent date
            var commentQ = from c in db.nursing_pca_comment
                             where c.pca_comment_type_id == typeID && c.pca_id == pcaID
                             group c by c.pca_comment_type_id into g
                             select g.OrderByDescending(t => t.date_comment_added).FirstOrDefault();

            ViewBag.CommentTypeName = db.nursing_pca_comment_type.FirstOrDefault(t => t.pca_comment_type_id == typeID).pca_comment_type_name;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;

            if (commentQ.FirstOrDefault() == null) {
                var emptyComment = new nursing_pca_comment();
                emptyComment.pca_id = (int)pcaID;
                emptyComment.pca_comment_type_id = (int)typeID;
                return View(emptyComment);
            }
            return View(commentQ.FirstOrDefault());
        }

        // POST: Comments/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "commentData")] FormCollection form) {
            int npcacID = int.Parse(form["pca_comment_id"]);
            nursing_pca_comment npcac = db.nursing_pca_comment.FirstOrDefault(m => m.pca_comment_id == npcacID);
            nursing_pca_record pca = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == npcac.pca_id);

            if (form["formButton"] == "Exit") {
                return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
            }

            if (ModelState.IsValid) {
                nursing_pca_comment_history npcachistory = new nursing_pca_comment_history();
                npcachistory.pca_comment_id = npcac.pca_comment_id;
                npcachistory.pca_comment_type_id = npcac.pca_comment_type_id;
                npcachistory.pca_id = npcac.pca_id;
                npcachistory.pca_comment = npcac.pca_comment;
                npcachistory.date_comment_original = npcac.date_comment_added;
                npcachistory.date_comment_modified = DateTime.Now;

                npcac.pca_comment = form["wdlExceptionInfo"];
                
                db.SaveChanges();

                switch (form["formButton"]) {
                    case "SaveList":
                        return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
                    case "SaveContinue":
                        int typeID = (int)npcac.pca_comment_type_id + 1;
                        if (typeID <= db.nursing_pca_comment_type.Count()) {
                            return RedirectToAction("CreateOrEdit", "Comments", new { pcaID = npcac.pca_id,  typeID = typeID});
                        }
                        else {
                            return RedirectToAction("Edit", "Comments", new { pcaID = npcac.pca_id, typeID = typeID });
                        }

                    default:
                        return View(npcac);
                }
            }
            return View(npcac);
        }

        // GET: Comments/Create
        public ActionResult Create(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.pcaID = pcaID;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;

            nursing_pca_comment_type npcact = db.nursing_pca_comment_type.FirstOrDefault(m => m.pca_comment_type_id == typeID);
            return View(npcact);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pca_comment_id, pca_id, pca_comment, date_comment_added")] nursing_pca_comment npcac, FormCollection form) {
            int pcaID = int.Parse(form["pcaID"]);
            nursing_pca_record pca = db.nursing_pca_record.FirstOrDefault(m => m.pca_id == pcaID);

            if (form["formButton"] == "Exit") {
                return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
            }

            if (ModelState.IsValid) {
                npcac.pca_id = pca.pca_id;
                npcac.pca_comment_type_id = int.Parse(form["pca_comment_type_id"]);
                npcac.pca_comment = form["commentData"];
                npcac.date_comment_added = DateTime.Now;
                db.nursing_pca_comment.Add(npcac);
                db.SaveChanges();

                switch (form["formButton"]) {
                    case "SaveList":
                        return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
                    case "SaveContinue":
                        int typeID = (int)npcac.pca_comment_type_id + 1;
                        if (typeID <= db.nursing_pca_comment_type.Count()) {
                            return RedirectToAction("CreateOrEdit", "Comments", new { pcaID = npcac.pca_id, typeID = typeID });
                        }
                        else {
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = pca.encounter_id });
                        }

                    default:
                        return View(npcac);
                }
            }

            ViewBag.pcaID = pcaID;
            ViewBag.CareSystems = db.nursing_care_system_assessment_type;
            ViewBag.PCAComments = db.nursing_pca_comment_type;
            return View(db.nursing_pca_comment_type.Find(npcac.pca_comment_type_id));
        }

        // GET: Comments/CreateOrEdit
        public ActionResult CreateOrEdit(int? pcaID, int? typeID) {
            if (pcaID == null || typeID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get the comment with matching typeID and pcaID
            var commentQ = from c in db.nursing_pca_comment
                             where c.pca_comment_type_id == typeID && c.pca_id == pcaID
                             group c by c.pca_comment_type_id into g
                             select g.FirstOrDefault();

            if (commentQ.FirstOrDefault() == null) {
                return RedirectToAction("Create", "Comments", new { pcaID = pcaID, typeID = typeID });
            }
            else {
                return RedirectToAction("Edit", "Comments", new { pcaID = pcaID, typeID = typeID });
            }
        }
    }
}