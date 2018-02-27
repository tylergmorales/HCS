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

namespace ispProject.Controllers
{
    public class EncounterController : Controller
    {
        private DataFacade facade = new DataFacade();
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();
        // GET: Encounter
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddEncounter(System.Web.Mvc.FormCollection form)
        {

            string cc = Request.Form["chiefComplaint"];
            int aType = Convert.ToInt32(Request.Form["aType"]);
            int doctor = Convert.ToInt32(Request.Form["doctor"]);
            string insurance = Request.Form["insurance"];
            int facility = Convert.ToInt32(Request.Form["facility"]);
           
            encounter pat = db.encounters.OrderByDescending(r => r.encounter_data_id).First();

            string encounterID = "0000000000000" + (pat.encounter_data_id + 1);

            encounterID = encounterID.Substring(encounterID.Length - 14, 14);



            encounter newEncounter = new encounter();
            newEncounter.encounter_id = encounterID;
            newEncounter.admission_date = DateTime.Now;
            newEncounter.chief_complaint = cc;
            newEncounter.admission_type_id = aType;
            newEncounter.attending_doctor_id = doctor;
            
            if (insurance == "0")
            {
                patient_insurance pi = new patient_insurance();                
                pi.patient_id = Convert.ToInt32(Request.Form["patientid"]);
                pi.insurance_id = 3;
                pi.individual_insurance_id = "Out Of Pocket";
                pi.date_added = DateTime.Now;
                db.patient_insurance.Add(pi);
                db.SaveChanges();

                newEncounter.patient_insurance_id = pi.patient_insurance_id;

            }
            else {
                patient_insurance pi = db.patient_insurance.Where(r => r.individual_insurance_id == insurance).FirstOrDefault();
                newEncounter.patient_insurance_id = pi.patient_insurance_id;
            }
         
            newEncounter.facility_id = facility;
            db.encounters.Add(newEncounter);
            db.SaveChanges();


            return Redirect("../IndividualEncounter/" + newEncounter.encounter_data_id);

        }
        public ActionResult AddEncounter(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            patient patient = db.patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Disabled")
            {
                ViewBag.isDisabled = true;
            }

            encounter newEncounter = new encounter();

            ViewBag.id = id;


            return View();
        }


        public ActionResult IndividualEncounter(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            encounter indiviEncounter = db.encounters.Where(r => r.encounter_data_id == id).FirstOrDefault();
            
            ViewBag.encounter = indiviEncounter.patient_insurance_id;
            ViewBag.encounter_data_id = indiviEncounter.encounter_data_id;
            patient_insurance pInsurance = db.patient_insurance.Where(r => r.patient_insurance_id == indiviEncounter.patient_insurance_id).FirstOrDefault();
            ViewBag.pi = pInsurance.patient_id;
            patient p = db.patients.Where(r => r.patient_id == pInsurance.patient_id).FirstOrDefault();

            ViewBag.patientID = p.patient_id;
            ViewBag.encounterID = indiviEncounter.encounter_id;
            ViewBag.patientMRN = p.medical_record_number;

            ViewBag.admissionDate = indiviEncounter.admission_date;
            ViewBag.dischargeDate = indiviEncounter.discharge_date;
            //Nursing group add new features here
            List<nursing_pca_record> pcaList = db.nursing_pca_record.Where(x => x.encounter_id == indiviEncounter.encounter_data_id).ToList();
            
                PCADetail pcaModel = new PCADetail();
                List<PCADetail> pcaModelList = new List<PCADetail>();
                if(pcaList != null && pcaList.Count > 0)
                {
                    pcaModelList = pcaList.Select(s => new PCADetail
                    {
                        pcaId = s.pca_id,
                        datetime = s.date_vitals_added,
                       
                        
                    }).OrderByDescending(s => s.datetime).ToList();
                }
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                //return View()
            

                return View(pcaModelList.ToPagedList(pageNumber, pageSize));

        }
        //Nursing added these controllers below
        // GET: Encounter
        public ActionResult CreateVital(int? id)
        {
            ViewBag.encounterdataid = db.encounters.Find(id).encounter_data_id;

            return View();
        }
        // POST: Encounter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVital(CreateVitals cr, int ? id, string formButton)
        {
            if(ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                encounter indiviEncounter = db.encounters.Find(id);

                ViewBag.encounterdataid = indiviEncounter.encounter_data_id;
                //nursing_pca_record pca_Record = db.nursing_pca_record.Where(x => x.encounter_id == indiviEncounter.encounter_data_id).FirstOrDefault();
                nursing_pca_record vi = new nursing_pca_record();
                vi.encounter_id = ViewBag.encounterdataid;
                vi.date_vitals_added = cr.datetime;
                vi.temperature = cr.temperature;
                vi.temp_route_type_id = cr.temp_route_value;
                vi.pulse = cr.pulse;
                vi.pulse_route_type_id = cr.pulse_route_value;
                vi.pulse_oximetry = cr.pulse_oximetry;
                vi.respiration = cr.respirations;
                vi.oxygen_flow = cr.oxygen_flow;
                vi.systolic_blood_pressure = cr.systolic_b_pressure;
                vi.diastolic_blood_pressure = cr.diastolic_b_pressure;
                vi.o_two_delivery_type_id = cr.o2_dev_method;
                vi.pain_scale_type_id = cr.pain_scale_value;
                vi.pain_level_actual = cr.pain_level_actual;
                vi.pain_level_goal = cr.pain_level__goal;
                nursing_care_system_assessment care = new nursing_care_system_assessment();
                care.care_system_assessment_type_id = 1;
                care.care_system_comment = cr.care_comment;
                care.wdl_ex = cr.wdl_ex;
                care.date_care_system_added = cr.datetime;
                vi.nursing_care_system_assessment = new List<nursing_care_system_assessment>();
                vi.nursing_care_system_assessment.Add(care);
                nursing_pca_comment com = new nursing_pca_comment();
                com.date_comment_added = cr.datetime;
                com.pca_comment = cr.comment;
                com.pca_comment_type_id = 1;
                vi.nursing_pca_comment = new List<nursing_pca_comment>();
                vi.nursing_pca_comment.Add(com);
                indiviEncounter.nursing_pca_record = new List<nursing_pca_record>();
                indiviEncounter.nursing_pca_record.Add(vi);
                db.nursing_pca_record.Add(vi);
               
                db.SaveChanges();
                switch (formButton) {
                    case "SaveList":
                        return Redirect("../IndividualEncounter/" + indiviEncounter.encounter_id);
                    case "SaveContinue":
                        return RedirectToAction("Create", "BodySystems", new { pcaID = vi.pca_id, typeID = 2 });
                    default:
                        return Redirect("../IndividualEncounter/" + indiviEncounter.encounter_id);
                }
            }
            return View();
        }
        public ActionResult ViewVitalDetail( int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nursing_pca_record pcaRecord = db.nursing_pca_record.Find(id);
            if(pcaRecord == null)
            {
                return HttpNotFound();
            }
            PCADetail detail = new PCADetail()
            {
                pcaId = pcaRecord.pca_id,
                encounterId = Convert.ToInt16(pcaRecord.encounter_id),
                datetime = pcaRecord.date_vitals_added,
                temperature = Convert.ToDecimal(pcaRecord.temperature),
                temp_route_type = facade.GetNursingTempRouteName(pcaRecord.temp_route_type_id),
                pulse = Convert.ToByte(pcaRecord.pulse),
                pulse_route_type = facade.GetNursingPulseRouteName(pcaRecord.pulse_route_type_id),
                pulse_oximetry = Convert.ToByte(pcaRecord.pulse_oximetry),
                respirations = Convert.ToByte(pcaRecord.respiration),
                oxygen_flow = pcaRecord.oxygen_flow,
                o2_dev_type = facade.GetO2DeliveryMethod(pcaRecord.o_two_delivery_type_id),
                systolic_b_pressure = Convert.ToByte(pcaRecord.systolic_blood_pressure),
                diastolic_b_pressure = Convert.ToByte(pcaRecord.diastolic_blood_pressure),
                pcaComment = facade.GetVitalComment(pcaRecord.pca_id),
                wdl = facade.GetWdlInfo(pcaRecord.pca_id),
                pain_scale_type = facade.GetPainScaleType(pcaRecord.pain_scale_type_id),
                pain_level_actual = Convert.ToByte(pcaRecord.pain_level_actual),
                pain_level__goal = Convert.ToByte(pcaRecord.pain_level_goal),
                painComment = facade.GetPainComment(pcaRecord.pca_id)

            };
            return View(detail);
        }

        [Authorize]
        public ActionResult UpdateVital(int? id)
        {
            
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //find pca record using pcaId
                nursing_pca_record pcaRecord = db.nursing_pca_record.Find(id);
                if (pcaRecord == null)
                {
                    return HttpNotFound();
                }

                ViewBag.CareSystems = db.nursing_care_system_assessment_type;

                UpdateVitals up = new UpdateVitals()
                {
                    pcaId = pcaRecord.pca_id,
                    encounterId = (int)pcaRecord.encounter_id,
                    datetime = pcaRecord.date_vitals_added,
                    temperature = Convert.ToDecimal(pcaRecord.temperature),
                    temp_route_type = facade.GetNursingTempRouteName(pcaRecord.temp_route_type_id),
                    pulse = Convert.ToByte(pcaRecord.pulse),
                    pulse_route_type = facade.GetNursingPulseRouteName(pcaRecord.pulse_route_type_id),
                    respirations = Convert.ToByte(pcaRecord.respiration),
                    pulse_oximetry = Convert.ToByte(pcaRecord.pulse_oximetry),
                    oxygen_flow = pcaRecord.oxygen_flow,
                    o2_type = facade.GetO2DeliveryMethod(pcaRecord.o_two_delivery_type_id),
                    systolic_b_pressure = Convert.ToByte(pcaRecord.systolic_blood_pressure),
                    diastolic_b_pressure = Convert.ToByte(pcaRecord.diastolic_blood_pressure),
                    comment = facade.GetVitalComment(pcaRecord.pca_id),
                    wdl_ex = facade.GetWdlInfo(pcaRecord.pca_id),
                    pain_scale_type = facade.GetPainScaleType(pcaRecord.pain_scale_type_id),
                    pain_level_actual = Convert.ToByte(pcaRecord.pain_level_actual),
                    pain_level__goal = Convert.ToByte(pcaRecord.pain_level_goal),
                    care_comment = facade.GetPainComment(pcaRecord.pca_id)

                };
                return View(up);

            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVital([Bind(Include = "datetime, temperature, temp_route_value, pulse, pulse_route_value, pulse_oximetry, " +
            "respirations, oxygen_flow, o2_dev_method, systolic_b_pressure, diastolic_b_pressure, " +
            "comment, wdl_ex, pain_scale_value, pain_level_actual, pain_level__goal, care_comment")]
        UpdateVitals update, int ? id, string formButton)
        {


            if (ModelState.IsValid)
            {
                    
                     nursing_pca_record pca = db.nursing_pca_record.Where(x => x.pca_id == id).FirstOrDefault();
                    nursing_pca_comment com = db.nursing_pca_comment.Where(s => s.pca_id == pca.pca_id).FirstOrDefault();
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pca.pca_id).FirstOrDefault();
                     encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();

                    ViewBag.encounterId = path.encounter_data_id;
                    //care = db.nursing_care_system_assessment.Select();
                    care.care_system_assessment_type_id = 1;
                    care.wdl_ex = Convert.ToBoolean(update.wdl_ex);
                    care.care_system_comment = update.care_comment;
                    care.date_care_system_added = update.datetime;
                    //nursing_pca_comment com = new nursing_pca_comment();
                    com.pca_comment = update.comment;
                    com.pca_comment_type_id = 1;
                    com.date_comment_added = update.datetime;
                    ViewBag.pca_id = pca.pca_id;
                    pca.date_vitals_added = update.datetime;
                    pca.temperature = update.temperature;
                    pca.temp_route_type_id = update.temp_route_value;
                    //update.temp_route_type = pca.temp_route_type_id.ToString();
                    pca.pulse = update.pulse;
                    pca.pulse_route_type_id = update.pulse_route_value;
                    pca.o_two_delivery_type_id = update.o2_dev_method;
                    pca.respiration = update.respirations;
                    pca.oxygen_flow = update.oxygen_flow;
                    pca.pulse_oximetry = update.pulse_oximetry;
                    pca.pain_scale_type_id = update.pain_scale_value;
                    pca.pain_level_actual = update.pain_level_actual;
                    pca.pain_level_goal = update.pain_level__goal;
                    nursing_pca_record_history historyPca = new nursing_pca_record_history();
                     historyPca.pca_id = ViewBag.pca_id;
                     historyPca.date_pca_record_orginal = pca.date_vitals_added;
                     historyPca.date_pca_record_modified = update.datetime;
                    historyPca.temperature = update.temperature;
                    historyPca.temp_route_type_id = update.temp_route_value;
                    historyPca.pulse = update.pulse;
                historyPca.pulse_route_type_id = update.pulse_route_value;
                historyPca.respiration = update.respirations;
                historyPca.pulse_oximetry = update.pulse_oximetry;
                historyPca.oxygen_flow = update.oxygen_flow;
                historyPca.pain_scale_type_id = update.pain_scale_value;
                historyPca.pain_level_actual = update.pain_level_actual;
                historyPca.pain_level_goal = update.pain_level__goal;
                db.nursing_pca_record_history.Add(historyPca);
                nursing_care_system_assessment_history historyCaresys = new nursing_care_system_assessment_history();
               
                ViewBag.careSysId = care.care_system_assessment_id;
                historyCaresys.pca_id = ViewBag.pca_id;
                historyCaresys.care_system_assessment_id = ViewBag.careSysId;
                historyCaresys.care_system_assessment_type_id = 1;
                historyCaresys.date_care_system_added = pca.date_vitals_added;
                historyCaresys.date_care_system_modified = update.datetime;
                historyCaresys.care_system_comment = update.care_comment;
                historyCaresys.wdl_ex = Convert.ToBoolean(update.wdl_ex);
                db.nursing_care_system_assessment_history.Add(historyCaresys);
                nursing_pca_comment_history historyCom = new nursing_pca_comment_history();
               
                ViewBag.commentId = com.pca_comment_id;
                historyCom.pca_id = ViewBag.pca_id;
                historyCom.pca_comment_id = ViewBag.commentId;
                historyCom.date_comment_original = pca.date_vitals_added;
                historyCom.date_comment_modified = update.datetime;
                historyCom.pca_comment = update.comment;
                historyCom.pca_comment_type_id = 1;
                db.nursing_pca_comment_history.Add(historyCom);
                    db.SaveChanges();

                switch (formButton) {
                    case "SaveList":
                        return Redirect("../IndividualEncounter/" + path.encounter_id);
                    case "SaveContinue":
                        return RedirectToAction("Edit", "BodySystems", new { pcaID = pca.pca_id, typeID = 2 });
                    default:
                        return Redirect("../IndividualEncounter/" + path.encounter_id);
                }
            }

            ViewBag.CareSystems = db.nursing_care_system_assessment_type;

            return View(update);

            

        }
     
    }


}