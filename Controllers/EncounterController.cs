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
        public ActionResult CreateVital()
        {

            return View();
        }
        // GET: Encounter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVital(CreateVitals cr, int ? id, string submit)
        {
            if(ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if(submit == "SaveAndGoBacktoList")
                {
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
                    return Redirect("../IndividualEncounter/" + indiviEncounter.encounter_id);
                }else if(submit == "SaveAndContinue")
                {
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
                    int pcaId1 = vi.pca_id;
                    ViewBag.pcaId = pcaId1;
                    //return Redirect("../CreateBodySystems/" + indiviEncounter.encounter_data_id);
                    return RedirectToAction("CreateBodySystems", "Encounter", new { id = indiviEncounter.encounter_data_id, pcaId = pcaId1 });
                   
                }
             

            }
            return View();
        }
        //public ActionResult ViewVitalDetail( int? id)
        //{
        //    if(id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    nursing_pca_record pcaRecord = db.nursing_pca_record.Find(id);
        //    if(pcaRecord == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    PCADetail detail = new PCADetail()
        //    {
        //        pcaId = pcaRecord.pca_id,
        //        encounterId = Convert.ToInt16(pcaRecord.encounter_id),
        //        datetime = pcaRecord.date_vitals_added,
        //        temperature = Convert.ToDecimal(pcaRecord.temperature),
        //        temp_route_type = facade.GetNursingTempRouteName(pcaRecord.temp_route_type_id),
        //        pulse = Convert.ToByte(pcaRecord.pulse),
        //        pulse_route_type = facade.GetNursingPulseRouteName(pcaRecord.pulse_route_type_id),
        //        pulse_oximetry = Convert.ToByte(pcaRecord.pulse_oximetry),
        //        respirations = Convert.ToByte(pcaRecord.respiration),
        //        oxygen_flow = pcaRecord.oxygen_flow,
        //        o2_dev_type = facade.GetO2DeliveryMethod(pcaRecord.o_two_delivery_type_id),
        //        systolic_b_pressure = Convert.ToByte(pcaRecord.systolic_blood_pressure),
        //        diastolic_b_pressure = Convert.ToByte(pcaRecord.diastolic_blood_pressure),
        //        pcaComment = facade.GetVitalComment(pcaRecord.pca_id),
        //        wdl = facade.GetWdlInfo(pcaRecord.pca_id),
        //        pain_scale_type = facade.GetPainScaleType(pcaRecord.pain_scale_type_id),
        //        pain_level_actual = Convert.ToByte(pcaRecord.pain_level_actual),
        //        pain_level__goal = Convert.ToByte(pcaRecord.pain_level_goal),
        //        painComment = facade.GetPainComment(pcaRecord.pca_id)

        //    };
        //    return View(detail);
        //}
        public ActionResult ViewPcaDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nursing_pca_record pcaRecord = db.nursing_pca_record.Find(id);
            ViewBag.pcaId = id;
            ViewBag.encounterId = pcaRecord.encounter_id;
            if (pcaRecord == null)
            {
                return HttpNotFound();
            }
            ViewPcaDetail view = new ViewPcaDetail()
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
                pcaComment1 = facade.GetPcaComment(pcaRecord.pca_id, 1),
                wdl1 = facade.GetWdlInfo(pcaRecord.pca_id),
                pain_scale_type = facade.GetPainScaleType(pcaRecord.pain_scale_type_id),
                pain_level_actual = Convert.ToByte(pcaRecord.pain_level_actual),
                pain_level__goal = Convert.ToByte(pcaRecord.pain_level_goal),
                bodysystemsComment1 = facade.GetPainComment(pcaRecord.pca_id),
                des1 = facade.GetWdlDes(1)
               

            };
            return View(view);
        }
        [ActionName("ViewPcaDetailNext")]
        public ActionResult ViewPcaDetailNext(int ? id)
        {
            ViewBag.pcaId = id;
            return View();
        }
        [ActionName("ViewPcaTypeDetailNext")]
        public ActionResult ViewPcaDetailNext(int? typeId, int ? pcaId)
        {

            nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
            encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
            ViewBag.encounterId = path.encounter_data_id;
            nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
            nursing_care_system_assessment nca = db.nursing_care_system_assessment.Where(m => m.pca_id == pcaId && m.care_system_assessment_type_id == typeId).FirstOrDefault();
            ViewBag.typeId = typeId;
            ViewBag.pcaId = pcaId;
            string message = "This Body System does not exist.";
           
            if (nca == null)
            {
                ViewBag.ErrorMessage = message;
                return View("ViewPcaDetailNext");
            }
            
                switch (typeId)
            {
                case 2:
                    ViewPcaDetail pcaDetailModel2 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel2.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 2);
                    pcaDetailModel2.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 2));
                    pcaDetailModel2.wdl = facade.GetBodysystemsWdl(nca.pca_id, 2);
                    return View("ViewPcaDetailNext", model: pcaDetailModel2);
                case 3:
                    ViewPcaDetail pcaDetailModel3 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel3.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 3);
                    pcaDetailModel3.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 3));
                    pcaDetailModel3.wdl = facade.GetBodysystemsWdl(nca.pca_id, 3);
                    return View("ViewPcaDetailNext", model: pcaDetailModel3);
                case 4:
                    ViewPcaDetail pcaDetailModel4 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel4.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 4);
                    pcaDetailModel4.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 4));
                    pcaDetailModel4.wdl = facade.GetBodysystemsWdl(nca.pca_id, 4);
                    return View("ViewPcaDetailNext", model: pcaDetailModel4);
                case 5:
                    ViewPcaDetail pcaDetailModel5 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel5.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 5);
                    pcaDetailModel5.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 5));
                    pcaDetailModel5.wdl = facade.GetBodysystemsWdl(nca.pca_id, 5);
                    return View("ViewPcaDetailNext", model: pcaDetailModel5);
                case 6:
                    ViewPcaDetail pcaDetailModel6 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel6.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 6);
                    pcaDetailModel6.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 6));
                    pcaDetailModel6.wdl = facade.GetBodysystemsWdl(nca.pca_id, 6);
                    return View("ViewPcaDetailNext", model: pcaDetailModel6);
                case 7:
                    ViewPcaDetail pcaDetailModel7 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel7.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 7);
                    pcaDetailModel7.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 7));
                    pcaDetailModel7.wdl = facade.GetBodysystemsWdl(nca.pca_id, 7);
                    return View("ViewPcaDetailNext", model: pcaDetailModel7);
                   
                case 8:
                    ViewPcaDetail pcaDetailModel8 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel8.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 8);
                    pcaDetailModel8.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 8));
                    pcaDetailModel8.wdl = facade.GetBodysystemsWdl(nca.pca_id, 8);
                    return View("ViewPcaDetailNext", model: pcaDetailModel8);
                case 9:
                    ViewPcaDetail pcaDetailModel9 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel9.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 9);
                    pcaDetailModel9.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 9));
                    pcaDetailModel9.wdl = facade.GetBodysystemsWdl(nca.pca_id, 9);
                    return View("ViewPcaDetailNext", model: pcaDetailModel9);
                case 10:
                    ViewPcaDetail pcaDetailModel10 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel10.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 10);
                    pcaDetailModel10.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 10));
                    pcaDetailModel10.wdl = facade.GetBodysystemsWdl(nca.pca_id, 10);
                    return View("ViewPcaDetailNext", model: pcaDetailModel10);
                case 11:
                    ViewPcaDetail pcaDetailModel11 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel11.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 11);
                    pcaDetailModel11.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 11));
                    pcaDetailModel11.wdl = facade.GetBodysystemsWdl(nca.pca_id, 11);
                    return View("ViewPcaDetailNext", model: pcaDetailModel11);
                case 12:
                    ViewPcaDetail pcaDetailModel12 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel12.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 12);
                    pcaDetailModel12.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 12));
                    pcaDetailModel12.wdl = facade.GetBodysystemsWdl(nca.pca_id, 12);
                    return View("ViewPcaDetailNext", model: pcaDetailModel12);
                case 13:
                    ViewPcaDetail pcaDetailModel13 = new ViewPcaDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel13.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 13);
                    pcaDetailModel13.bodysystemsDateAdded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 13));
                    pcaDetailModel13.wdl = facade.GetBodysystemsWdl(nca.pca_id, 13);
                    return View("ViewPcaDetailNext", model: pcaDetailModel13);
            }

            return View("ViewPcaDetailNext");



            
        }
        [ActionName("ViewPcaCommentTypeDetailNext")]
        public ActionResult ViewPcaDetailNext(int typeId, int pcaId)
        {
            nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
            encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
            ViewBag.encounterId = path.encounter_data_id;
            nursing_pca_comment_type ct = db.nursing_pca_comment_type.Find(typeId);
            //nursing_pca_record pca = db.nursing_pca_record.Find(pcaId);
            nursing_pca_comment pcm = db.nursing_pca_comment.Where(cm => cm.pca_id == pcaId && cm.pca_comment_type_id == typeId).FirstOrDefault();
            ViewPcaDetail pcaDetailModel = new ViewPcaDetail();
            ViewBag.typeId = typeId;
            ViewBag.pcaId = pcaId;
            string message = "This PCA Comment does not exist.";

            if (pcm == null)
            {
                ViewBag.ErrorMessage = message;
                return View("ViewPcaDetailNext");
            }

            switch (typeId)
            {
                case 2:
                    
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    pcaDetailModel.pcaComment = facade.GetPcaComment(pcm.pca_id, 2);
                    pcaDetailModel.pcaCommentDateAdded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 2));
                    return View("ViewPcacommentDetailNext", model: pcaDetailModel);
                case 3:
                    UpdatePcaComment uc1 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    pcaDetailModel.pcaComment = facade.GetPcaComment(pcm.pca_id, 3);
                    pcaDetailModel.pcaCommentDateAdded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 3));
                    return View("ViewPcacommentDetailNext", model: pcaDetailModel);
                case 4:
                    UpdatePcaComment uc2 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    pcaDetailModel.pcaComment = facade.GetPcaComment(pcm.pca_id, 4);
                    pcaDetailModel.pcaCommentDateAdded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 4));
                    return View("ViewPcacommentDetailNext", model: pcaDetailModel);
                case 5:
                    UpdatePcaComment uc3 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    pcaDetailModel.pcaComment = facade.GetPcaComment(pcm.pca_id, 5);
                    pcaDetailModel.pcaCommentDateAdded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 5));
                    return View("ViewPcacommentDetailNext", model: pcaDetailModel);

            }
            return View("ViewPcacommentDetailNext");
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
            UpdateVitals up = new UpdateVitals()
            {
                pcaId = pcaRecord.pca_id,
                datetime = pcaRecord.date_vitals_added,
                temperature = Convert.ToDecimal(pcaRecord.temperature),
                temp_route_type = facade.GetNursingTempRouteName(pcaRecord.temp_route_type_id),
                temp_route_value = Convert.ToInt16(pcaRecord.temp_route_type_id),
                pulse = Convert.ToByte(pcaRecord.pulse),
                pulse_route_type = facade.GetNursingPulseRouteName(pcaRecord.pulse_route_type_id),
                pulse_route_value = Convert.ToInt16(pcaRecord.pulse_route_type_id),
                respirations = Convert.ToByte(pcaRecord.respiration),
                pulse_oximetry = Convert.ToByte(pcaRecord.pulse_oximetry),
                oxygen_flow = pcaRecord.oxygen_flow,
                o2_type = facade.GetO2DeliveryMethod(pcaRecord.o_two_delivery_type_id),
                o2_dev_method = Convert.ToInt16(pcaRecord.o_two_delivery_type_id),
                systolic_b_pressure = Convert.ToByte(pcaRecord.systolic_blood_pressure),
                diastolic_b_pressure = Convert.ToByte(pcaRecord.diastolic_blood_pressure),
                comment = facade.GetVitalComment(pcaRecord.pca_id),
                wdl_ex = facade.GetWdlInfo(pcaRecord.pca_id),
                pain_scale_type = facade.GetPainScaleType(pcaRecord.pain_scale_type_id),
                pain_scale_value = Convert.ToInt16(pcaRecord.pain_scale_type_id),
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
        UpdateVitals update, int ? id, string submit)
        {
           
                if (ModelState.IsValid)
            {
                if (submit == "SaveAndGoBacktoList")
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
                    historyPca.encounter_id = ViewBag.encounterId;
                    historyPca.date_vitals_added = Convert.ToDateTime(facade.GetPcaDatimeAdded(pca.pca_id));
                    historyPca.date_pca_record_orginal = Convert.ToDateTime(facade.GetPcaDatimeAdded(pca.pca_id));
                    historyPca.date_pca_record_modified = update.datetime;
                    historyPca.temperature = update.temperature;
                    historyPca.temp_route_type_id = update.temp_route_value;
                    historyPca.pulse = update.pulse;
                    historyPca.pulse_route_type_id = update.pulse_route_value;
                    historyPca.respiration = update.respirations;
                    historyPca.pulse_oximetry = update.pulse_oximetry;
                    historyPca.oxygen_flow = update.oxygen_flow;
                    historyPca.o_two_delivery_type_id = update.o2_dev_method;
                    historyPca.systolic_blood_pressure = update.systolic_b_pressure;
                    historyPca.diastolic_blood_pressure = update.diastolic_b_pressure;
                    historyPca.pain_scale_type_id = update.pain_scale_value;
                    historyPca.pain_level_actual = update.pain_level_actual;
                    historyPca.pain_level_goal = update.pain_level__goal;
                    db.nursing_pca_record_history.Add(historyPca);
                    nursing_care_system_assessment_history historyCaresys = new nursing_care_system_assessment_history();

                    ViewBag.careSysId = care.care_system_assessment_id;
                    historyCaresys.pca_id = ViewBag.pca_id;
                    historyCaresys.care_system_assessment_id = ViewBag.careSysId;
                    historyCaresys.care_system_assessment_type_id = 1;
                    historyCaresys.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 1));
                    historyCaresys.date_care_system_modified = update.datetime;
                    historyCaresys.care_system_comment = update.care_comment;
                    historyCaresys.wdl_ex = Convert.ToBoolean(update.wdl_ex);
                    db.nursing_care_system_assessment_history.Add(historyCaresys);
                    nursing_pca_comment_history historyCom = new nursing_pca_comment_history();

                    ViewBag.commentId = com.pca_comment_id;
                    historyCom.pca_id = ViewBag.pca_id;
                    historyCom.pca_comment_id = ViewBag.commentId;
                    historyCom.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(com.pca_id, 1));
                    historyCom.date_comment_modified = update.datetime;
                    historyCom.pca_comment = update.comment;
                    historyCom.pca_comment_type_id = 1;
                    db.nursing_pca_comment_history.Add(historyCom);
                    db.SaveChanges();

                    return Redirect("../IndividualEncounter/" + path.encounter_data_id);
                }else if (submit == "SaveAndContinue")
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
                    historyPca.encounter_id = ViewBag.encounterId;
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

                    return Redirect("../UpdateBodysystems/" + pca.pca_id);
                }
                    
            }
                return View(update);

            

        }
        // Adding Bodysystems C.R.U.D functions
        [ActionName("CreateBodySystems")]
        public ActionResult CreateBodySystems(int ? id, int ? pcaId)
        {


            //encounter en = db.encounters.Find(id);
            //nursing_pca_record pca = db.nursing_pca_record.Find(pcaId);

            //nursing_pca_record na = db.nursing_pca_record.Where(p => p.encounter_id == en.encounter_data_id).FirstOrDefault();

            ViewBag.pcaId = pcaId;
            //nursing_care_system_assessment cs = db.nursing_care_system_assessment.Where(c => c.pca_id == na.pca_id).FirstOrDefault();
            return View(); 
        }

        [ActionName("CreateBodySystemsType")]
        public ActionResult CreateBodySystems( int typeId, int pcaId )
        {
            //nursing_pca_record pca = db.nursing_pca_record.Where(pa => pa.pca_id == pcaId).FirstOrDefault();
            nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
            nursing_care_system_assessment nca = db.nursing_care_system_assessment.Where(ca => ca.pca_id == pcaId && ca.care_system_assessment_type_id == typeId).FirstOrDefault();
            ViewBag.pcaId = pcaId;
            ViewBag.typeId = typeId;
            if (nca != null)
            {

                return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = ViewBag.typeId, pcaId = ViewBag.pcaId });
            }
            else if (nca == null)
            {
                TempData["TempModel"] = "This Body System does not exist. Please creating new Body System before editing.";
            }
            

            switch (typeId)
            {
                case 2:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 3:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 4:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 5:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 6:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 7:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 8:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 9:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 10:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 11:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 12:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
                case 13:
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    ViewBag.datetime = DateTime.Now;
                    return View("CreateBodySystems");
            }
            
            
            
            return View("CreateBodySystems"); 
        }
        [ActionName("CreateBodySystemsType")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBodySystems(CreateBodySys crB, int ? typeId, int ? pcaId, string save)
        {
            if (ModelState.IsValid)
            {
               
                if (save == "SaveAndExit")
                {
                    nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pca.pca_id && h.care_system_assessment_type_id == typeId).FirstOrDefault();
                    
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    //if)
                    //{
                    //    return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = ViewBag.typeId, pcaId = ViewBag.pcaId });
                    //}
                   
                    switch (typeId)
                    {

                        case 2:
                            nursing_care_system_assessment nr = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr.date_care_system_added = DateTime.Now;
                            nr.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr.care_system_comment = crB.care_sys_comment;
                            nr.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 3:
                            nursing_care_system_assessment nr1 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr1.date_care_system_added = DateTime.Now;
                            nr1.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr1.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr1.care_system_comment = crB.care_sys_comment;
                            nr1.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr1);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 4:
                            nursing_care_system_assessment nr2 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr2.date_care_system_added = DateTime.Now;
                            nr2.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr2.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr2.care_system_comment = crB.care_sys_comment;
                            nr2.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr2);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });


                        case 5:
                            nursing_care_system_assessment nr3 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr3.date_care_system_added = DateTime.Now;
                            nr3.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr3.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr3.care_system_comment = crB.care_sys_comment;
                            nr3.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr3);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 6:
                            nursing_care_system_assessment nr4 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr4.date_care_system_added = DateTime.Now;
                            nr4.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr4.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr4.care_system_comment = crB.care_sys_comment;
                            nr4.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr4);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 7:
                            nursing_care_system_assessment nr5 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr5.date_care_system_added = DateTime.Now;
                            nr5.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr5.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr5.care_system_comment = crB.care_sys_comment;
                            nr5.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr5);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 8:
                            nursing_care_system_assessment nr6 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr6.date_care_system_added = DateTime.Now;
                            nr6.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr6.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr6.care_system_comment = crB.care_sys_comment;
                            nr6.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr6);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 9:
                            nursing_care_system_assessment nr7 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr7.date_care_system_added = DateTime.Now;
                            nr7.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr7.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr7.care_system_comment = crB.care_sys_comment;
                            nr7.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr7);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 10:
                            nursing_care_system_assessment nr8 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr8.date_care_system_added = DateTime.Now;
                            nr8.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr8.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr8.care_system_comment = crB.care_sys_comment;
                            nr8.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr8);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 11:
                            nursing_care_system_assessment nr9 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr9.date_care_system_added = DateTime.Now;
                            nr9.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr9.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr9.care_system_comment = crB.care_sys_comment;
                            nr9.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr9);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 12:
                            nursing_care_system_assessment nr10 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr10.date_care_system_added = DateTime.Now;
                            nr10.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr10.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr10.care_system_comment = crB.care_sys_comment;
                            nr10.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr10);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                        case 13:
                            nursing_care_system_assessment nr11 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr11.date_care_system_added = DateTime.Now;
                            nr11.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr11.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = ViewBag.datetime;
                            nr11.care_system_comment = crB.care_sys_comment;
                            nr11.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr11);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                    }


                    }else if (save == "SaveAndContinue")
                {
                    nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pca.pca_id && h.care_system_assessment_type_id == typeId).FirstOrDefault();

                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.typeId = typeId;
                    ViewBag.care_sys_type_id = typeId;
                    if (care != null)
                    {
                        return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = ViewBag.typeId, pcaId = ViewBag.pcaId });
                    }
                    switch (typeId)
                    {

                        case 2:
                            nursing_care_system_assessment nr = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr.date_care_system_added = DateTime.Now;
                            nr.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr.care_system_comment = crB.care_sys_comment;
                            nr.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 3, pcaId = ViewBag.pcaId, });

                        case 3:
                            nursing_care_system_assessment nr1 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr1.date_care_system_added = DateTime.Now;
                            nr1.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr1.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr1.care_system_comment = crB.care_sys_comment;
                            nr1.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr1);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 4, pcaId = ViewBag.pcaId, });
                        case 4:
                            nursing_care_system_assessment nr2 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr2.date_care_system_added = DateTime.Now;
                            nr2.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr2.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr2.care_system_comment = crB.care_sys_comment;
                            nr2.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr2);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 5, pcaId = ViewBag.pcaId, });
                        case 5:
                            nursing_care_system_assessment nr3 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr3.date_care_system_added = DateTime.Now;
                            nr3.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr3.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr3.care_system_comment = crB.care_sys_comment;
                            nr3.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr3);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 6, pcaId = ViewBag.pcaId, });

                        case 6:
                            nursing_care_system_assessment nr4 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr4.date_care_system_added = DateTime.Now;
                            nr4.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr4.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr4.care_system_comment = crB.care_sys_comment;
                            nr4.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr4);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 7, pcaId = ViewBag.pcaId, });

                        case 7:
                            nursing_care_system_assessment nr5 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr5.date_care_system_added = DateTime.Now;
                            nr5.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr5.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr5.care_system_comment = crB.care_sys_comment;
                            nr5.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr5);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 8, pcaId = ViewBag.pcaId, });

                        case 8:
                            nursing_care_system_assessment nr6 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr6.date_care_system_added = DateTime.Now;
                            nr6.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr6.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr6.care_system_comment = crB.care_sys_comment;
                            nr6.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr6);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 9, pcaId = ViewBag.pcaId, });

                        case 9:
                            nursing_care_system_assessment nr7 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr7.date_care_system_added = DateTime.Now;
                            nr7.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr7.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr7.care_system_comment = crB.care_sys_comment;
                            nr7.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr7);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 10, pcaId = ViewBag.pcaId, });

                        case 10:
                            nursing_care_system_assessment nr8 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr8.date_care_system_added = DateTime.Now;
                            nr8.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr8.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr8.care_system_comment = crB.care_sys_comment;
                            nr8.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr8);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 11, pcaId = ViewBag.pcaId, });
                        case 11:
                            nursing_care_system_assessment nr9 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr9.date_care_system_added = DateTime.Now;
                            nr9.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr9.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr9.care_system_comment = crB.care_sys_comment;
                            nr9.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr9);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 12, pcaId = ViewBag.pcaId, });

                        case 12:
                            nursing_care_system_assessment nr10 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr10.date_care_system_added = DateTime.Now;
                            nr10.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr10.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr10.care_system_comment = crB.care_sys_comment;
                            nr10.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr10);
                            db.SaveChanges();
                            return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = 13, pcaId = ViewBag.pcaId, });

                        case 13:
                            nursing_care_system_assessment nr11 = new nursing_care_system_assessment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.care_sys_type_id = typeId;
                            nr11.date_care_system_added = DateTime.Now;
                            nr11.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nr11.pca_id = ViewBag.pcaId;
                            crB.datetimeadded = DateTime.Now;
                            nr11.care_system_comment = crB.care_sys_comment;
                            nr11.wdl_ex = crB.wdl_ex;
                            db.nursing_care_system_assessment.Add(nr11);
                            db.SaveChanges();
                            return RedirectToAction("CreatePcaComment", "Encounter", new { typeId = 2, pcaId = ViewBag.pcaId, });

                    }
                }

            }
           
            return View();
        }
        [ActionName("CreatePcaComment")]
        public ActionResult CreatePcaComment(int typeId, int pcaId)
        {
            nursing_pca_comment_type ct = db.nursing_pca_comment_type.Find(typeId);
          
            switch (typeId)
            {
                case 2:
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    ViewBag.datetime = DateTime.Now;
                    return View();
                case 3:
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    ViewBag.datetime = DateTime.Now;
                    return View();
                case 4:
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    ViewBag.datetime = DateTime.Now;
                    return View();
                case 5:
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    ViewBag.datetime = DateTime.Now;
                    return View();

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePcaComment(CreatePcaComment crP, int? typeId, int? pcaId, string save)
        {
            if (ModelState.IsValid)
            {
                if (save == "SaveAndExit")
                {
                    nursing_pca_comment_type pt = db.nursing_pca_comment_type.Find(typeId);
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pca.pca_id && h.care_system_assessment_type_id == typeId).FirstOrDefault();
                    nursing_pca_comment pcm = db.nursing_pca_comment.Where(pc => pc.pca_id == pcaId && pc.pca_comment_type_id == typeId).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                   
                    switch (typeId)
                         {
                        case 2:
                            nursing_pca_comment pc = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pc.date_comment_added;
                            pc.pca_comment = crP.comment;
                            pc.pca_comment_type_id = ViewBag.comment_type_id;
                            pc.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 3:
                            nursing_pca_comment pc1 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc1.date_comment_added = DateTime.Now; ;
                            ViewBag.datetime = pc1.date_comment_added;
                            pc1.pca_comment = crP.comment;
                            pc1.pca_comment_type_id = ViewBag.comment_type_id;
                            pc1.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc1);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 4:
                            nursing_pca_comment pc2 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc2.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pc2.date_comment_added;
                            pc2.pca_comment = crP.comment;
                            pc2.pca_comment_type_id = ViewBag.comment_type_id;
                            pc2.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc2);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 5:
                            nursing_pca_comment pc3 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc3.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pc3.date_comment_added;
                            pc3.pca_comment = crP.comment;
                            pc3.pca_comment_type_id = ViewBag.comment_type_id;
                            pc3.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc3);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                    }

                }else if (save == "SaveAndContinue")
                    {
                    nursing_pca_comment_type pt = db.nursing_pca_comment_type.Find(typeId);
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pca.pca_id && h.care_system_assessment_type_id == typeId).FirstOrDefault();
                    nursing_pca_comment pcm = db.nursing_pca_comment.Where(pc => pc.pca_id == pcaId && pc.pca_comment_type_id == typeId).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    switch (typeId)
                    {
                        case 2:
                            nursing_pca_comment pc = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc.date_comment_added = DateTime.Now;
                            pc.pca_comment = crP.comment;
                            pc.pca_comment_type_id = ViewBag.comment_type_id;
                            pc.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc);
                            db.SaveChanges();
                            return RedirectToAction("CreatePcaComment", "Encounter", new { typeId = 3, pcaId = ViewBag.pcaId, });
                        case 3:
                            nursing_pca_comment pc1 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc1.date_comment_added = DateTime.Now;
                            pc1.pca_comment = crP.comment;
                            pc1.pca_comment_type_id = ViewBag.comment_type_id;
                            pc1.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc1);
                            db.SaveChanges();
                            return RedirectToAction("CreatePcaComment", "Encounter", new { typeId = 4, pcaId = ViewBag.pcaId, });
                        case 4:
                            nursing_pca_comment pc2 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc2.date_comment_added = DateTime.Now;
                            pc2.pca_comment = crP.comment;
                            pc2.pca_comment_type_id = ViewBag.comment_type_id;
                            pc2.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc2);
                            db.SaveChanges();
                            return RedirectToAction("CreatePcaComment", "Encounter", new { typeId = 5, pcaId = ViewBag.pcaId, });
                        case 5:
                            nursing_pca_comment pc3 = new nursing_pca_comment();
                            ViewBag.pcaId = pcaId;
                            ViewBag.comment_type_id = typeId;
                            pc3.date_comment_added = DateTime.Now;
                            pc3.pca_comment = crP.comment;
                            pc3.pca_comment_type_id = ViewBag.comment_type_id;
                            pc3.pca_id = ViewBag.pcaId;
                            db.nursing_pca_comment.Add(pc3);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                    }
                }
            }
                return View();
        }
        
        [ActionName("UpdateBodysystems")]
        public ActionResult UpdateBodysystems(int? id)
        {
            //encounter en = db.encounters.Where(e => e.encounter_data_id == id).FirstOrDefault();
            nursing_care_system_assessment na = db.nursing_care_system_assessment.Where(n => n.pca_id == id).FirstOrDefault();
            ViewBag.pcaId = na.pca_id;
            //nursing_care_system_assessment cs = db.nursing_care_system_assessment.Where(c => c.pca_id == na.pca_id).FirstOrDefault();


            return View();
        }
        [ActionName("UpdateBodysystemsType")]
        public ActionResult UpdateBodysystems(int typeId, int pcaId)
        {

            nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
            //nursing_pca_record pca = db.nursing_pca_record.Find(pcaId);
            nursing_care_system_assessment nca = db.nursing_care_system_assessment.Where(m => m.pca_id == pcaId && m.care_system_assessment_type_id == typeId).FirstOrDefault();
            //UpdateBodysystem u = new UpdateBodysystem();
            //u.care_sys_comment = nca.care_system_comment;
            //u.datetimeadded = nca.date_care_system_added;
            //u.wdl_ex = Convert.ToBoolean(nca.wdl_ex);
            ViewBag.typeId = typeId;
            ViewBag.pcaId = pcaId;
            
           
            //string message = "This Body System does not exist. Please creating new Body System before editing.";
           
            if (nca == null)
            {
                TempData["TempModel"] = "This Body System does not exist. Please creating new Body System before editing.";
                //ViewBag.ErrorMessage = message;
                return RedirectToAction("CreateBodySystemsType", "Encounter", new { typeId = ViewBag.typeId, pcaId = ViewBag.pcaId } );
               
            }
            switch (typeId)
            {
                case 2:
                    UpdateBodysystem u = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 2);
                    u.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 2));
                    u.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 2));
                    return View("UpdateBodysystems", model: u );
                case 3:
                    UpdateBodysystem u1 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u1.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 3);
                    u1.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id,3));
                    u1.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id,3));
                    return View("UpdateBodysystems", model: u1);
                case 4:
                    UpdateBodysystem u2 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u2.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id,4);
                    u2.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 4));
                    u2.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id,4));
                    return View("UpdateBodysystems", model: u2);
                case 5:
                    UpdateBodysystem u3 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u3.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id,5);
                    u3.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 5));
                    u3.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id,5));
                    return View("UpdateBodysystems", model: u3);
                case 6:
                    UpdateBodysystem u4 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u4.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id,6);
                    u4.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 6));
                    u4.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id,6));
                    return View("UpdateBodysystems", model: u4);
                case 7:
                    UpdateBodysystem u5 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u5.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 7);
                    u5.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 7));
                    u5.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id,7));
                    return View("UpdateBodysystems", model: u5);
                case 8:
                    UpdateBodysystem u6 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u6.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 8);
                    u6.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 8));
                    u6.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 8));
                    return View("UpdateBodysystems", model: u6);
                case 9:
                    UpdateBodysystem u7 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u7.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 9);
                    u7.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 9));
                    u7.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 9));
                    return View("UpdateBodysystems", model: u7);
                case 10:
                    UpdateBodysystem u8 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u8.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 10);
                    u8.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 10));
                    u8.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 10));
                    return View("UpdateBodysystems", model: u8);
                case 11:
                    UpdateBodysystem u9 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u9.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 11);
                    u9.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 11));
                    u9.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 11));
                    return View("UpdateBodysystems", model: u9);
                case 12:
                    UpdateBodysystem u10 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u10.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 12);
                    u10.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 12));
                    u10.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 12));
                    return View("UpdateBodysystems", model: u10);
                case 13:
                    UpdateBodysystem u11 = new UpdateBodysystem();
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    u11.care_sys_comment = facade.GeBodySystemsComment(nca.pca_id, 13);
                    u11.datetimeadded = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 13));
                    u11.wdl_ex = Convert.ToBoolean(facade.GetBodysystemsWdl(nca.pca_id, 13));
                    return View("UpdateBodysystems", model: u11);
            }
            
            return View("UpdateBodysystems");
           
        }
        [ActionName("UpdateBodySystemsType")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateBodysystems(UpdateBodysystem upB, int? typeId, int? pcaId, string save)
        {
            if (ModelState.IsValid)
            {
                if (save == "SaveAndExit")
                {
                    

                    nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pcaId && h.care_system_assessment_type_id == typeId).FirstOrDefault();
                    
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == care.pca_id).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    switch (typeId)
                    {
                        case 2:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh = new nursing_care_system_assessment_history();
                            nh.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 2));
                            nh.date_care_system_modified = DateTime.Now;
                            nh.care_system_comment = upB.care_sys_comment;
                            nh.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh.care_system_assessment_id = ViewBag.careSysId;
                            nh.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 3:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh1 = new nursing_care_system_assessment_history();
                            nh1.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 3));
                            nh1.date_care_system_modified = DateTime.Now;
                            nh1.care_system_comment = upB.care_sys_comment;
                            nh1.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh1.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh1.care_system_assessment_id = ViewBag.careSysId;
                            nh1.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh1);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 4:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh4 = new nursing_care_system_assessment_history();
                            nh4.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 4));
                            nh4.date_care_system_modified = DateTime.Now;
                            nh4.care_system_comment = upB.care_sys_comment;
                            nh4.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh4.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh4.care_system_assessment_id = ViewBag.careSysId;
                            nh4.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh4);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 5:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh5 = new nursing_care_system_assessment_history();
                            nh5.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 5));
                            nh5.date_care_system_modified = DateTime.Now;
                            nh5.care_system_comment = upB.care_sys_comment;
                            nh5.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh5.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh5.care_system_assessment_id = ViewBag.careSysId;
                            nh5.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh5);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 6:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh6 = new nursing_care_system_assessment_history();
                            nh6.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 6));
                            nh6.date_care_system_modified = DateTime.Now;
                            nh6.care_system_comment = upB.care_sys_comment;
                            nh6.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh6.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh6.care_system_assessment_id = ViewBag.careSysId;
                            nh6.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh6);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 7:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh7 = new nursing_care_system_assessment_history();
                            nh7.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 7));
                            nh7.date_care_system_modified = DateTime.Now;
                            nh7.care_system_comment = upB.care_sys_comment;
                            nh7.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh7.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh7.care_system_assessment_id = ViewBag.careSysId;
                            nh7.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh7);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 8:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh8 = new nursing_care_system_assessment_history();
                            nh8.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 8));
                            nh8.date_care_system_modified = DateTime.Now;
                            nh8.care_system_comment = upB.care_sys_comment;
                            nh8.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh8.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh8.care_system_assessment_id = ViewBag.careSysId;
                            nh8.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh8);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 9:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh9 = new nursing_care_system_assessment_history();
                            nh9.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 9));
                            nh9.date_care_system_modified = DateTime.Now;
                            nh9.care_system_comment = upB.care_sys_comment;
                            nh9.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh9.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh9.care_system_assessment_id = ViewBag.careSysId;
                            nh9.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh9);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 10:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh10 = new nursing_care_system_assessment_history();
                            nh10.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 10));
                            nh10.date_care_system_modified = DateTime.Now;
                            nh10.care_system_comment = upB.care_sys_comment;
                            nh10.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh10.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh10.care_system_assessment_id = ViewBag.careSysId;
                            nh10.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh10);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 11:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh11 = new nursing_care_system_assessment_history();
                            nh11.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 11));
                            nh11.date_care_system_modified = DateTime.Now;
                            nh11.care_system_comment = upB.care_sys_comment;
                            nh11.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh11.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh11.care_system_assessment_id = ViewBag.careSysId;
                            nh11.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh11);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 12:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh12 = new nursing_care_system_assessment_history();
                            nh12.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 12));
                            nh12.date_care_system_modified = DateTime.Now;
                            nh12.care_system_comment = upB.care_sys_comment;
                            nh12.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh12.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh12.care_system_assessment_id = ViewBag.careSysId;
                            nh12.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh12);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 13:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            care.pca_id = ViewBag.pcaId;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh13 = new nursing_care_system_assessment_history();
                            nh13.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 13));
                            nh13.date_care_system_modified = DateTime.Now;
                            nh13.care_system_comment = upB.care_sys_comment;
                            nh13.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh13.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh13.care_system_assessment_id = ViewBag.careSysId;
                            nh13.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh13);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                           
                    }
                 }
                else if (save == "SaveAndContinue")
                {
                    nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
                    nursing_care_system_assessment care = db.nursing_care_system_assessment.Where(h => h.pca_id == pcaId && h.care_system_assessment_type_id == typeId).FirstOrDefault();
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == care.pca_id).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    switch (typeId)
                    {
                        case 2:
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh = new nursing_care_system_assessment_history();
                            nh.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 2));
                            nh.date_care_system_modified = DateTime.Now;
                            nh.care_system_comment = upB.care_sys_comment;
                            nh.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh.care_system_assessment_id = ViewBag.careSysId;
                            nh.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 3, pcaId = ViewBag.pcaId });
                        case 3:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh1 = new nursing_care_system_assessment_history();
                            nh1.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 3));
                            nh1.date_care_system_modified = DateTime.Now;
                            nh1.care_system_comment = upB.care_sys_comment;
                            nh1.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh1.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh1.care_system_assessment_id = ViewBag.careSysId;
                            nh1.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh1);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 4, pcaId = ViewBag.pcaId });
                        case 4:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh4 = new nursing_care_system_assessment_history();
                            nh4.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 4));
                            nh4.date_care_system_modified = DateTime.Now;
                            nh4.care_system_comment = upB.care_sys_comment;
                            nh4.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh4.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh4.care_system_assessment_id = ViewBag.careSysId;
                            nh4.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh4);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 5, pcaId = ViewBag.pcaId });
                        case 5:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh5 = new nursing_care_system_assessment_history();
                            nh5.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 5));
                            nh5.date_care_system_modified = DateTime.Now;
                            nh5.care_system_comment = upB.care_sys_comment;
                            nh5.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh5.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh5.care_system_assessment_id = ViewBag.careSysId;
                            nh5.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh5);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 6, pcaId = ViewBag.pcaId });
                        case 6:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh6 = new nursing_care_system_assessment_history();
                            nh6.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 6));
                            nh6.date_care_system_modified = DateTime.Now;
                            nh6.care_system_comment = upB.care_sys_comment;
                            nh6.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh6.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh6.care_system_assessment_id = ViewBag.careSysId;
                            nh6.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh6);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 7, pcaId = ViewBag.pcaId });

                        case 7:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh7 = new nursing_care_system_assessment_history();
                            nh7.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 7));
                            nh7.date_care_system_modified = DateTime.Now;
                            nh7.care_system_comment = upB.care_sys_comment;
                            nh7.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh7.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh7.care_system_assessment_id = ViewBag.careSysId;
                            nh7.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh7);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 8, pcaId = ViewBag.pcaId });
                        case 8:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh8 = new nursing_care_system_assessment_history();
                            nh8.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 8));
                            nh8.date_care_system_modified = DateTime.Now;
                            nh8.care_system_comment = upB.care_sys_comment;
                            nh8.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh8.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh8.care_system_assessment_id = ViewBag.careSysId;
                            nh8.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh8);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 9, pcaId = ViewBag.pcaId });
                        case 9:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh9 = new nursing_care_system_assessment_history();
                            nh9.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 9));
                            nh9.date_care_system_modified = DateTime.Now;
                            nh9.care_system_comment = upB.care_sys_comment;
                            nh9.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh9.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh9.care_system_assessment_id = ViewBag.careSysId;
                            nh9.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh9);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 10, pcaId = ViewBag.pcaId });
                        case 10:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh10 = new nursing_care_system_assessment_history();
                            nh10.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 10));
                            nh10.date_care_system_modified = DateTime.Now;
                            nh10.care_system_comment = upB.care_sys_comment;
                            nh10.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh10.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh10.care_system_assessment_id = ViewBag.careSysId;
                            nh10.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh10);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 11, pcaId = ViewBag.pcaId });
                        case 11:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh11 = new nursing_care_system_assessment_history();
                            nh11.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 11));
                            nh11.date_care_system_modified = DateTime.Now;
                            nh11.care_system_comment = upB.care_sys_comment;
                            nh11.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh11.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh11.care_system_assessment_id = ViewBag.careSysId;
                            nh11.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh11);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 12, pcaId = ViewBag.pcaId });
                        case 12:
                            
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh12 = new nursing_care_system_assessment_history();
                            nh12.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 12));
                            nh12.date_care_system_modified = DateTime.Now;
                            nh12.care_system_comment = upB.care_sys_comment;
                            nh12.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh12.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh12.care_system_assessment_id = ViewBag.careSysId;
                            nh12.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh12);
                            db.SaveChanges();
                            return RedirectToAction("UpdateBodySystemsType", "Encounter", new { typeId = 13, pcaId = ViewBag.pcaId });
                        case 13:
                           
                            care.date_care_system_added = DateTime.Now;
                            care.care_system_comment = upB.care_sys_comment;
                            care.wdl_ex = upB.wdl_ex;
                            nursing_care_system_assessment_history nh13 = new nursing_care_system_assessment_history();
                            nh13.date_care_system_added = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(care.pca_id, 13));
                            nh13.date_care_system_modified = DateTime.Now;
                            nh13.care_system_comment = upB.care_sys_comment;
                            nh13.care_system_assessment_type_id = ViewBag.care_sys_type_id;
                            nh13.pca_id = ViewBag.pcaId;
                            ViewBag.careSysId = care.care_system_assessment_id;
                            nh13.care_system_assessment_id = ViewBag.careSysId;
                            nh13.wdl_ex = upB.wdl_ex;
                            db.nursing_care_system_assessment_history.Add(nh13);
                            db.SaveChanges();
                            return RedirectToAction("UpdatePcaComment", "Encounter", new { typeId = 2, pcaId = ViewBag.pcaId, });

                    }
                }

            }
                return View();
        }
        public ActionResult UpdatePcaComment(int typeId, int pcaId)
        {
            nursing_pca_comment_type ct = db.nursing_pca_comment_type.Find(typeId);
            //nursing_pca_record pca = db.nursing_pca_record.Find(pcaId);
            nursing_pca_comment pcm = db.nursing_pca_comment.Where(cm => cm.pca_id == pcaId && cm.pca_comment_type_id == typeId).FirstOrDefault();
            ViewBag.pcaId = pcaId;
            ViewBag.comment_type_id = typeId;
            string message = "This PCA Comment does not exist. Please creating new PCA Comment before editing.";
            if (pcm == null)
            {
                ViewBag.ErrorMessage = message;
                return View("UpdatePcaComment");

            }

            switch (typeId)
            {
                case 2:
                    UpdatePcaComment uc = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc.comment = facade.GetPcaComment(pcm.pca_id,2);
                    uc.datetimeadded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 2));
                    return View("UpdatePcaComment", model: uc);
                case 3:
                    UpdatePcaComment uc1 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc1.comment = facade.GetPcaComment(pcm.pca_id, 3);
                    uc1.datetimeadded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 3));
                    return View("UpdatePcaComment", model: uc1);
                case 4:
                    UpdatePcaComment uc2 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc2.comment = facade.GetPcaComment(pcm.pca_id, 4);
                    uc2.datetimeadded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 4));
                    return View("UpdatePcaComment", model: uc2);
                case 5:
                    UpdatePcaComment uc3 = new UpdatePcaComment();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc3.comment = facade.GetPcaComment(pcm.pca_id, 5);
                    uc3.datetimeadded = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 5));
                    return View("UpdatePcaComment", model: uc3);

            }
            return View("UpdatePcaComment");
        }
        [ActionName("UpdatePcaComment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePcaComment(UpdatePcaComment udP, int? typeId, int? pcaId, string save)
        {
            if (ModelState.IsValid)
            {
                if (save == "SaveAndExit")
                {
                    nursing_pca_comment_type pt = db.nursing_pca_comment_type.Find(typeId);
                    nursing_pca_comment pcm = db.nursing_pca_comment.Where(p => p.pca_id == pcaId && p.pca_comment_type_id == typeId).FirstOrDefault();
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcm.pca_id).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.commentId = pcm.pca_comment_id;

                    switch (typeId)
                    {
                        case 2:
                           
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph = new nursing_pca_comment_history();
                            ph.pca_id = ViewBag.pcaId;
                            ph.pca_comment_type_id = ViewBag.comment_type_id;
                            ph.pca_comment_id = ViewBag.commentId;
                            ph.pca_comment = udP.comment;
                            ph.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 2));
                            ph.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 3:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph3 = new nursing_pca_comment_history();
                            ph3.pca_id = ViewBag.pcaId;
                            ph3.pca_comment_type_id = ViewBag.comment_type_id;
                            ph3.pca_comment_id = ViewBag.commentId;
                            ph3.pca_comment = udP.comment;
                            ph3.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 3));
                            ph3.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph3);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 4:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph4 = new nursing_pca_comment_history();
                            ph4.pca_id = ViewBag.pcaId;
                            ph4.pca_comment_type_id = ViewBag.comment_type_id;
                            ph4.pca_comment_id = ViewBag.commentId;
                            ph4.pca_comment = udP.comment;
                            ph4.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 4));
                            ph4.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph4);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });
                        case 5:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph5 = new nursing_pca_comment_history();
                            ph5.pca_id = ViewBag.pcaId;
                            ph5.pca_comment_type_id = ViewBag.comment_type_id;
                            ph5.pca_comment_id = ViewBag.commentId;
                            ph5.pca_comment = udP.comment;
                            ph5.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 5));
                            ph5.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph5);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                    }

                }
                else if (save == "SaveAndContinue")
                {
                    nursing_pca_comment_type pt = db.nursing_pca_comment_type.Find(typeId);
                    nursing_pca_comment pcm = db.nursing_pca_comment.Where(p => p.pca_id == pcaId && p.pca_comment_type_id == typeId).FirstOrDefault();
                    nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcm.pca_id).FirstOrDefault();
                    encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
                    ViewBag.encounterId = path.encounter_data_id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.commentId = pcm.pca_comment_id;

                    switch (typeId)
                    {
                        case 2:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph = new nursing_pca_comment_history();
                            ph.pca_id = ViewBag.pcaId;
                            ph.pca_comment_type_id = ViewBag.comment_type_id;
                            ph.pca_comment_id = ViewBag.commentId;
                            ph.pca_comment = udP.comment;
                            ph.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 2));
                            ph.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph);
                            db.SaveChanges();
                            return RedirectToAction("UpdatePcaComment", "Encounter", new { typeId = 3, pcaId = ViewBag.pcaId, });
                        case 3:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph3 = new nursing_pca_comment_history();
                            ph3.pca_id = ViewBag.pcaId;
                            ph3.pca_comment_type_id = ViewBag.comment_type_id;
                            ph3.pca_comment_id = ViewBag.commentId;
                            ph3.pca_comment = udP.comment;
                            ph3.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 3));
                            ph3.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph3);
                            db.SaveChanges();
                            return RedirectToAction("UpdatePcaComment", "Encounter", new { typeId = 4, pcaId = ViewBag.pcaId, });
                        case 4:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph4 = new nursing_pca_comment_history();
                            ph4.pca_id = ViewBag.pcaId;
                            ph4.pca_comment_type_id = ViewBag.comment_type_id;
                            ph4.pca_comment_id = ViewBag.commentId;
                            ph4.pca_comment = udP.comment;
                            ph4.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 4));
                            ph4.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph4);
                            db.SaveChanges();
                            return RedirectToAction("UpdatePcaComment", "Encounter", new { typeId = 5, pcaId = ViewBag.pcaId, });
                        case 5:
                            pcm.date_comment_added = DateTime.Now;
                            ViewBag.datetime = pcm.date_comment_added;
                            pcm.pca_comment = udP.comment;
                            nursing_pca_comment_history ph5 = new nursing_pca_comment_history();
                            ph5.pca_id = ViewBag.pcaId;
                            ph5.pca_comment_type_id = ViewBag.comment_type_id;
                            ph5.pca_comment_id = ViewBag.commentId;
                            ph5.pca_comment = udP.comment;
                            ph5.date_comment_original = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 5));
                            ph5.date_comment_modified = DateTime.Now;
                            db.nursing_pca_comment_history.Add(ph5);
                            db.SaveChanges();
                            return RedirectToAction("IndividualEncounter", "Encounter", new { id = path.encounter_id });

                    }
                }
            }
            return View();
        }

        //Get List of History Records
        public ActionResult ListOfHistoryRecords(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nursing_pca_record npr = db.nursing_pca_record.Where(x => x.pca_id == id).
                FirstOrDefault();
            encounter encounter = db.encounters.Where(enc => enc.encounter_data_id == npr.encounter_id).FirstOrDefault();
            ViewBag.encounterId = encounter.encounter_data_id;

            List<nursing_pca_record_history> pcaList = db.nursing_pca_record_history.Where
                (y => y.pca_id == npr.pca_id).ToList();

            PCAHistoryDetail pcaModel = new PCAHistoryDetail();
            List<PCAHistoryDetail> pcaModelList = new List<PCAHistoryDetail>();
            if (pcaList != null && pcaList.Count > 0)
            {
                pcaModelList = pcaList.Select(s => new PCAHistoryDetail

                {
                    pcaHistoryId = s.pca_record_history_id,
                    pcaId = s.pca_id,
                    datetime = s.date_pca_record_modified


                }).OrderByDescending(s => s.datetime).ToList();
            }
            else
            {
                return RedirectToAction("RecordError", "Encounter", new { id = ViewBag.encounterId });
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            //return View()


            return View(pcaModelList.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult RecordError(int ? id)
        {
            ViewBag.encounterId = id;
            return View();
        }

        
        public ActionResult IndividualHistoryRecord(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nursing_pca_record_history pcaRecord = db.nursing_pca_record_history.SingleOrDefault(m => m.pca_record_history_id == id);
            nursing_pca_record p = db.nursing_pca_record.Where(p1 => p1.pca_id == pcaRecord.pca_id).FirstOrDefault();
            ViewBag.pcaId = p.pca_id;
            encounter en = db.encounters.Where(e1 => e1.encounter_data_id == pcaRecord.encounter_id).FirstOrDefault();
            ViewBag.encounterId = en.encounter_data_id;
            if (pcaRecord == null)
            {
                return HttpNotFound();
            }
            PCAHistoryDetail detail = new PCAHistoryDetail()
            {
                pcaHistoryId = pcaRecord.pca_record_history_id,
                pcaId = pcaRecord.pca_id,
                encounterId = pcaRecord.encounter_id,
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

        [ActionName("ViewPcaHistoryDetailNext")]
        public ActionResult ViewPcaHistoryDetailNext(int? id,int ? pcaId)
        {
            ViewBag.pcaHistoryId = id;
            ViewBag.pcaId = pcaId;
            return View();
        }
        [ActionName("ViewPcaTypeHistoryDetailNext")]
        public ActionResult ViewPcaHistoryDetailNext(int? typeId, int? id, int? pcaId)
        {

            nursing_pca_record_history pcaRecord = db.nursing_pca_record_history.SingleOrDefault(m => m.pca_record_history_id == id);
            nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
            encounter path = db.encounters.Where(en => en.encounter_data_id == pcaRecord.encounter_id).FirstOrDefault();
            ViewBag.encounterId = path.encounter_data_id;
            nursing_care_system_assessment_type nt = db.nursing_care_system_assessment_type.Find(typeId);
            nursing_care_system_assessment_history nca = db.nursing_care_system_assessment_history.Where(m => m.pca_id == pcaId && m.care_system_assessment_type_id == typeId).FirstOrDefault();
            ViewBag.typeId = typeId;
            ViewBag.pcaHistoryId = id;
            ViewBag.pcaId = pcaId;
            string message= "This Body System does not contain any historical data.";
            if (nca == null)
            {
                ViewBag.ErrorMessage = message;
                 return View("ViewPcaHistoryDetailNext");
            }

            switch (typeId)
            {
                case 2:
                    PCAHistoryDetail pcaDetailModel2 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel2.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 2);
                    pcaDetailModel2.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 2));
                    pcaDetailModel2.wdl = facade.GetBodysystemsWdl(nca.pca_id, 2);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel2);
                case 3:
                    PCAHistoryDetail pcaDetailModel3 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel3.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 3);
                    pcaDetailModel3.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 3));
                    pcaDetailModel3.wdl = facade.GetBodysystemsWdl(nca.pca_id, 3);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel3);
                case 4:
                    PCAHistoryDetail pcaDetailModel4 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel4.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 4);
                    pcaDetailModel4.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 4));
                    pcaDetailModel4.wdl = facade.GetBodysystemsWdl(nca.pca_id, 4);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel4);
                case 5:
                    PCAHistoryDetail pcaDetailModel5 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel5.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 5);
                    pcaDetailModel5.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 5));
                    pcaDetailModel5.wdl = facade.GetBodysystemsWdl(nca.pca_id, 5);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel5);
                case 6:
                    PCAHistoryDetail pcaDetailModel6 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel6.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 6);
                    pcaDetailModel6.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 6));
                    pcaDetailModel6.wdl = facade.GetBodysystemsWdl(nca.pca_id, 6);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel6);
                case 7:
                    PCAHistoryDetail pcaDetailModel7 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel7.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 7);
                    pcaDetailModel7.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 7));
                    pcaDetailModel7.wdl = facade.GetBodysystemsWdl(nca.pca_id, 7);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel7);

                case 8:
                    PCAHistoryDetail pcaDetailModel8 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel8.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 8);
                    pcaDetailModel8.dateModified= Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 8));
                    pcaDetailModel8.wdl = facade.GetBodysystemsWdl(nca.pca_id, 8);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel8);
                case 9:
                    PCAHistoryDetail pcaDetailModel9 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel9.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 9);
                    pcaDetailModel9.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 9));
                    pcaDetailModel9.wdl = facade.GetBodysystemsWdl(nca.pca_id, 9);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel9);
                case 10:
                    PCAHistoryDetail pcaDetailModel10 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel10.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 10);
                    pcaDetailModel10.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 10));
                    pcaDetailModel10.wdl = facade.GetBodysystemsWdl(nca.pca_id, 10);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel10);
                case 11:
                    PCAHistoryDetail pcaDetailModel11 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel11.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 11);
                    pcaDetailModel11.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 11));
                    pcaDetailModel11.wdl = facade.GetBodysystemsWdl(nca.pca_id, 11);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel11);
                case 12:
                    PCAHistoryDetail pcaDetailModel12 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel12.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 12);
                    pcaDetailModel12.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 12));
                    pcaDetailModel12.wdl = facade.GetBodysystemsWdl(nca.pca_id, 12);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel12);
                case 13:
                    PCAHistoryDetail pcaDetailModel13 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.care_sys_type_id = typeId;
                    ViewBag.care_sys_type_name = nt.care_system_assessment_type_name;
                    ViewBag.care_sys_des = nt.wdl_description;
                    pcaDetailModel13.bodysystemsComment = facade.GeBodySystemsComment(nca.pca_id, 13);
                    pcaDetailModel13.dateModified = Convert.ToDateTime(facade.GetBodySystemsDatetimeAdded(nca.pca_id, 13));
                    pcaDetailModel13.wdl = facade.GetBodysystemsWdl(nca.pca_id, 13);
                    return View("ViewPcaHistoryDetailNext", model: pcaDetailModel13);
            }

            return View("ViewPcaHistoryDetailNext");

        }
        [ActionName("ViewPcaCommentHistoryTypeDetailNext")]
        public ActionResult ViewPcaCommentHistoryDetailNext(int? typeId, int? id, int? pcaId)
        {
          
           
            nursing_pca_record_history pcaRecord = db.nursing_pca_record_history.SingleOrDefault(m => m.pca_record_history_id == id);
            nursing_pca_record pca = db.nursing_pca_record.Where(pc => pc.pca_id == pcaId).FirstOrDefault();
            encounter path = db.encounters.Where(en => en.encounter_data_id == pca.encounter_id).FirstOrDefault();
            ViewBag.encounterId = path.encounter_data_id;
            nursing_pca_comment_type ct = db.nursing_pca_comment_type.Find(typeId);
            nursing_pca_comment_history pcm = db.nursing_pca_comment_history.Where(cm => cm.pca_id == pcaId && cm.pca_comment_type_id == typeId).FirstOrDefault();
            PCAHistoryDetail pcaDetailModel = new PCAHistoryDetail();
            ViewBag.typeId = typeId;
            ViewBag.pcaId = pcaId;
            ViewBag.pcaHistoryId = id;
            string message = "This PCA Comment does not contain any historical data.";
            if (pcm == null)
            {
                ViewBag.ErrorMessage = message;
                return View("ViewPcaCommentHistoryDetailNext");
            }

            switch (typeId)
            {
                case 2:
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    pcaDetailModel.pcaComment = facade.GetPcaComment(pcm.pca_id, 2);
                    pcaDetailModel.dateModified = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 2));
                    return View("ViewPcaCommentHistoryDetailNext", model: pcaDetailModel);
                case 3:
                    PCAHistoryDetail uc1 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc1.pcaComment = facade.GetPcaComment(pcm.pca_id, 3);
                    uc1.dateModified = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 3));
                    return View("ViewPcaCommentHistoryDetailNext", model: pcaDetailModel);
                case 4:
                    PCAHistoryDetail uc2 = new PCAHistoryDetail();
                    ViewBag.pcaHistoryId = id;
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc2.pcaComment = facade.GetPcaComment(pcm.pca_id, 4);
                    uc2.dateModified = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 4));
                    return View("ViewPcaCommentHistoryDetailNext", model: pcaDetailModel);
                case 5:
                    PCAHistoryDetail uc3 = new PCAHistoryDetail();
                    ViewBag.pcaId = pcaId;
                    ViewBag.comment_type_id = typeId;
                    ViewBag.comment_type_name = ct.pca_comment_type_name;
                    uc3.pcaComment = facade.GetPcaComment(pcm.pca_id, 5);
                    uc3.dateModified = Convert.ToDateTime(facade.GetPcaCommentDatetimeAdded(pcm.pca_id, 5));
                    return View("ViewPcaCommentHistoryDetailNext", model: pcaDetailModel);

            }
            return View("ViewPcaCommentHistoryDetailNext");
        }


    }


    }