using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ispProject.Models;
using NLog;
using ispProject.Security;
using System.Windows.Forms;
using System.Data.Entity.Validation;

namespace ispProject.Controllers
{
    [Authorize]
    public class patientsController : Controller
    {

        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: patients
        public ActionResult Index()
        {
            return View(db.zz_patient.ToList());
        }

        /*NEW STUFF*/

        public ActionResult PatientCheckIn()
        {
            ViewBag.mrn = "Patients Checked In";
            return View(db.encounter_main.Where(r => r.discharge_date == null).ToList());
        }

        public ActionResult PatientEncounters(string mrn)
        {

            patient patient = db.patients.Where(r => r.medical_record_number == mrn).First();

            patient_name pName = db.patient_name.Where(r => r.patient_id == patient.patient_id && r.patient_name_type_id == 1).First();

            List<patient_insurance> pi = db.patient_insurance.Where(r => r.patient_id == patient.patient_id).ToList();

            List<encounter_main> encounterHistory = new List<encounter_main>();

            foreach (patient_insurance pin in pi)
            {
                List<encounter_main> e = db.encounter_main.Where(r => r.individual_insurance_id == pin.individual_insurance_id).ToList();

                foreach (encounter_main a in e)
                {
                    encounterHistory.Add(a);
                }
            }
            ViewBag.id = patient.patient_id;
            ViewBag.mrn = patient.medical_record_number;
            ViewBag.last = pName.last_name;
            ViewBag.first = pName.first_name;
            return View(encounterHistory);
        }

        /*End NEW STUFF*/

        // GET: patients/Details/5
        public ActionResult Details(int? id)
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
            //List<zz_record> records = db.zz_record.Where(r => r.patientId == patient.patientId).ToList();
            //ViewBag.records = records;
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title == "Disabled")
            {
                ViewBag.isDisabled = true;
            }
            ViewBag.patientId = patient.medical_record_number;
            //records[0].birthRecord.dateCreated.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult DetailsUpdateAll(System.Web.Mvc.FormCollection form)
        {

            string mrn = Request.Form["mrn"];
            patient p = db.patients.Where(r => r.medical_record_number == mrn).FirstOrDefault();
            user account = db.users.Find(UserAccount.GetUserID());

            int patientID = p.patient_id;
            //Tables
            List<patient_name> pn = db.patient_name.Where(r => r.patient_id == patientID).ToList();
            List<patient_family> pf = db.patient_family.Where(r => r.patient_id == patientID).ToList();
            patient_birth_information pbi = db.patient_birth_information.Where(r => r.patient_id == patientID).FirstOrDefault();
            List<patient_address> pa = db.patient_address.Where(r => r.patient_id == patientID).ToList();
            List<patient_insurance> pi = db.patient_insurance.Where(r => r.patient_id == patientID).ToList();
            //Form inputs
            //PrimaryName
            string fName = Request.Form["firstName"];
            string mName = Request.Form["middleName"];
            string lName = Request.Form["lastName"];
            //NewAlias
            string AfName = Request.Form["AfirstName"];
            string AmName = Request.Form["AmiddleName"];
            string AlName = Request.Form["AlastName"];


            //Work out Patient Names
            if (pn.Any())
            {
                //Find Primary Name
                patient_name foundPrimary = db.patient_name.Where(r => r.patient_id == patientID && r.patient_name_type_id == 1).FirstOrDefault();
                if (foundPrimary != null)
                {//if the patient has a primary name (Which they should)
                    if (foundPrimary.first_name != fName || foundPrimary.middle_name != mName || foundPrimary.last_name != lName)
                    {//check if the primary name changed
                        foundPrimary.patient_name_type_id = 2; //change primary name to an alias.
                        foundPrimary.date_edited = DateTime.Now;
                        foundPrimary.edited_by = account.userId;
                        db.Entry(foundPrimary).State = EntityState.Modified;
                        patient_name newPrimary = new patient_name();
                        newPrimary.patient_name_type_id = 1;
                        newPrimary.patient_id = patientID;
                        newPrimary.first_name = fName;
                        newPrimary.middle_name = mName;
                        newPrimary.last_name = lName;
                        newPrimary.date_added = DateTime.Now;
                        db.patient_name.Add(newPrimary);
                        db.SaveChanges();
                    }
                    if (AfName.Length > 1 && AlName.Length > 1)
                    {//Add an Alias
                        patient_name newAlias = new patient_name();
                        newAlias.patient_name_type_id = 2;
                        newAlias.patient_id = patientID;
                        newAlias.first_name = AfName;
                        newAlias.middle_name = AmName;
                        newAlias.last_name = AlName;
                        newAlias.date_added = DateTime.Now;
                        db.patient_name.Add(newAlias);
                        db.SaveChanges();
                    }
                }
            }
            //update any other General Info
            string motherMaidenName = Request.Form["mothersMaidenName"];
            string sex = Request.Form["sex"];
            string gender = Request.Form["gender"];
            int maritalStatus = Convert.ToInt32(Request.Form["maritalStatus"]);
            int race = Convert.ToInt32(Request.Form["race"]);
            int ethnicity = Convert.ToInt32(Request.Form["ethnicity"]);
            int changed = 0;
            if (p.mother_maiden_name != motherMaidenName)
            {
                p.mother_maiden_name = motherMaidenName; changed++;
            }
            if (p.patient_ethnicity_id != ethnicity)
            {
                p.patient_ethnicity_id = ethnicity; changed++;
            }
            if (p.marital_status_id != maritalStatus)
            {
                p.marital_status_id = maritalStatus; changed++;
            }
            if (p.patient_race_id != race)
            {
                p.patient_race_id = race; changed++;
            }
            if (changed > 0)//if there were any changes, update patient record.
            {
                p.date_edited = DateTime.Now;
                p.edited_by = account.userId;
                db.Entry(p).State = EntityState.Modified;
            }
            string address = Request.Form["address"];
            string address2 = Request.Form["address2"];
            string city = Request.Form["city"];
            string state = Request.Form["state"];
            string zip = Request.Form["zip"];

            string addressBilling = Request.Form["addressBilling"];
            string address2Billing = Request.Form["address2Billing"];
            string cityBilling = Request.Form["cityBilling"];
            string stateBilling = Request.Form["stateBilling"];
            string zipBilling = Request.Form["zipBilling"];
            changed = 0;
            if (pa.Any())
            {
                patient_address primaryAddress = db.patient_address.Where(r => r.address_type_id == 1).FirstOrDefault();
                if (primaryAddress != null)
                {
                    if (primaryAddress.street_address_one != address || primaryAddress.street_address_two != address2)
                    {
                        changed++;
                        primaryAddress.address_type_id = 4;
                        primaryAddress.date_edited = DateTime.Now;
                        primaryAddress.edited_by = account.userId;
                        db.Entry(primaryAddress).State = EntityState.Modified;
                        patient_address newPrimary = new patient_address();
                        newPrimary.patient_id = patientID;
                        newPrimary.street_address_one = address;
                        newPrimary.street_address_two = address2;
                        newPrimary.city = city;
                        newPrimary.state = state;
                        newPrimary.zip = zip;
                        newPrimary.address_type_id = 1;
                        newPrimary.date_added = DateTime.Now;
                        db.patient_address.Add(newPrimary);

                    }
                }
                else {
                    changed++;
                    patient_address newPrimary = new patient_address();
                    newPrimary.patient_id = patientID;
                    newPrimary.street_address_one = address;
                    newPrimary.street_address_two = address2;
                    newPrimary.city = city;
                    newPrimary.state = state;
                    newPrimary.zip = zip;
                    newPrimary.address_type_id = 1;
                    newPrimary.date_added = DateTime.Now;
                    db.patient_address.Add(newPrimary);

                }
                patient_address primaryAddressBilling = db.patient_address.Where(r => r.address_type_id == 2).FirstOrDefault();
                if (primaryAddressBilling != null)
                {

                    if (primaryAddressBilling.street_address_one != addressBilling || primaryAddressBilling.street_address_two != address2Billing)
                    {
                        changed++;

                        patient_address newPrimaryBilling = new patient_address();
                        newPrimaryBilling.patient_id = patientID;
                        newPrimaryBilling.street_address_one = addressBilling;
                        newPrimaryBilling.street_address_two = address2Billing;
                        newPrimaryBilling.city = cityBilling;
                        newPrimaryBilling.state = stateBilling;
                        newPrimaryBilling.zip = zipBilling;
                        newPrimaryBilling.address_type_id = 2;
                        newPrimaryBilling.date_added = DateTime.Now;
                        db.patient_address.Add(newPrimaryBilling);
                        primaryAddressBilling.address_type_id = 4;
                        primaryAddressBilling.date_edited = DateTime.Now;
                        primaryAddressBilling.edited_by = account.userId;
                        db.Entry(primaryAddressBilling).State = EntityState.Modified;

                    }
                }
                else {
                    changed++;
                    patient_address newPrimaryBilling = new patient_address();
                    newPrimaryBilling.patient_id = patientID;
                    newPrimaryBilling.street_address_one = addressBilling;
                    newPrimaryBilling.street_address_two = address2Billing;
                    newPrimaryBilling.city = cityBilling;
                    newPrimaryBilling.state = stateBilling;
                    newPrimaryBilling.zip = zipBilling;
                    newPrimaryBilling.address_type_id = 2;
                    newPrimaryBilling.date_added = DateTime.Now;
                    db.patient_address.Add(newPrimaryBilling);

                }




            }
            else {//If no addresses exist add them
                if (address.Length > 1 || address2.Length > 1 || city.Length > 1 || state.Length > 1 || zip.Length > 1)
                {
                    changed++;
                    patient_address newAddress = new patient_address();
                    newAddress.patient_id = patientID;
                    newAddress.street_address_one = address;
                    newAddress.street_address_two = address2;
                    newAddress.city = city;
                    newAddress.state = state;
                    newAddress.zip = zip;
                    newAddress.address_type_id = 1;
                    newAddress.date_added = DateTime.Now;
                    db.patient_address.Add(newAddress);
                }
                if (addressBilling.Length > 1 || address2Billing.Length > 1 || cityBilling.Length > 1 || stateBilling.Length > 1 || zipBilling.Length > 1)
                {
                    changed++;
                    patient_address newAddressBilling = new patient_address();
                    newAddressBilling.patient_id = patientID;
                    newAddressBilling.street_address_one = addressBilling;
                    newAddressBilling.street_address_two = address2Billing;
                    newAddressBilling.city = cityBilling;
                    newAddressBilling.state = stateBilling;
                    newAddressBilling.zip = zipBilling;
                    newAddressBilling.address_type_id = 2;
                    newAddressBilling.date_added = DateTime.Now;
                    db.patient_address.Add(newAddressBilling);
                }


            }
            if (changed > 0)
            {//If address is added or updated, save the changes.

                db.SaveChanges();
            }

            ViewBag.patientId = mrn;
            return View("Details", new { id = mrn });

        }

        [HttpPost]
        public ActionResult AddInsurance(System.Web.Mvc.FormCollection form)
        {



            string subscriberNumber = Request.Form["subscriberNumber"]; //individual_insurance_id
            string insuranceCompany = Request.Form["insuranceCompany"]; //insurance_id
            int patientID = Convert.ToInt32(Request.Form["patientID"]);

            patient p = db.patients.Where(r => r.patient_id == patientID).FirstOrDefault();
            patient_insurance patientInsure = new patient_insurance();
            patientInsure.patient_id = patientID;
            patientInsure.insurance_id = Convert.ToInt32(insuranceCompany);
            patientInsure.individual_insurance_id = subscriberNumber;

            db.patient_insurance.Add(patientInsure);
            db.SaveChanges();

            return Redirect("Details/" + p.medical_record_number);


        }



        // GET: patients/Create
        public ActionResult Create()
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title != "Disabled")
            {
                return View();
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateNew(patient p, System.Web.Mvc.FormCollection form)
        {
            try
            {
                patient pat = db.patients.OrderByDescending(r => r.patient_id).First();

                string mrn = "00000" + (pat.patient_id + 1);

                mrn = mrn.Substring(mrn.Length - 6, 6);

                user account = db.users.Find(UserAccount.GetUserID());

                p.date_created = DateTime.Now;
                p.social_security_number = form["ssn"];
                p.medical_record_number = mrn;
                HttpCookie aCookie = Request.Cookies["userId"];
                p.edited_by = Convert.ToInt32(Server.HtmlEncode(aCookie.Value));

                db.patients.Add(p);

                db.SaveChanges();

                logger.Info("User " + account.firstName + " " + account.lastName + " created patient: " + p.medical_record_number);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ViewBag.ssn = p.social_security_number;
            ViewBag.mrn = p.medical_record_number;

            return View("Create");
        }

        // POST: patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult CreateGeneral(System.Web.Mvc.FormCollection form)
        {
            string mrn = Request.Form["mrn"];
            patient p = db.patients.Where(r => r.medical_record_number == mrn).FirstOrDefault();
            try
            {
                user account = db.users.Find(UserAccount.GetUserID());

                int patientID = p.patient_id;

                patient_name pn = new patient_name();
                patient_family mother = new patient_family();
                patient_family father = new patient_family();
                patient_birth_information pbi = db.patient_birth_information.Where(r => r.patient_id == patientID).FirstOrDefault();

                // Update patient's patient table

                p.gender_id = Convert.ToInt32(Request.Form["gender"]);
                p.mother_maiden_name = Request.Form["mothersMaidenName"];
                p.date_edited = DateTime.Now;
                p.marital_status_id = Convert.ToInt32(Request.Form["maritalStatus"]);
                p.patient_ethnicity_id = Convert.ToInt32(Request.Form["ethnicity"]);
                p.patient_race_id = Convert.ToInt32(Request.Form["race"]);
                if (Request.Cookies["userId"] != null)
                {
                    HttpCookie aCookie = Request.Cookies["userId"];
                    p.edited_by = Convert.ToInt32(Server.HtmlEncode(aCookie.Value));
                }
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                //update patient's name table
                //Primary Name
                pn.first_name = Request.Form["firstName"];
                pn.middle_name = Request.Form["middleName"];
                pn.last_name = Request.Form["lastName"];
                pn.patient_id = patientID;
                pn.patient_name_type_id = 1; //Find this in db later
                pn.date_added = DateTime.Now;

                db.patient_name.Add(pn);
                db.SaveChanges();
                string aliasFirstName = Request.Form["AfirstName"];
                string aliasMiddleName = Request.Form["AmiddleName"];
                string aliasLastName = Request.Form["AlastName"];

                if (aliasFirstName.Length > 1 && aliasLastName.Length > 1)
                {
                    patient_name pna = new patient_name();
                    pna.first_name = aliasFirstName;
                    pna.middle_name = aliasMiddleName;
                    pna.last_name = aliasLastName;
                    pna.patient_id = patientID;
                    pna.patient_name_type_id = 2;
                    pna.date_added = DateTime.Now;

                    db.patient_name.Add(pna);
                    db.SaveChanges();
                }
                if (pbi != null) { }
                else {
                    patient_birth_information newPBI = new patient_birth_information();
                    newPBI.birth_date = DateTime.Parse(Request.Form["dob"]).Date;
                    newPBI.patient_id = patientID;
                    newPBI.date_added = DateTime.Now;
                    db.patient_birth_information.Add(newPBI);
                    db.SaveChanges();
                }

                //logger.Info("User " + account.firstName + " " + account.lastName + " created patient: " + p.medical_record_number);
                // return View("Details", new { id = p.medical_record_number });
                return Redirect("Details/" + p.medical_record_number);
            }
            catch (DbEntityValidationException dbEx)
            {

                //return View("Details", new { id = p.medical_record_number });
                return Redirect("Details/" + p.medical_record_number);
            }



        }

        // GET: patients/Edit/5
        public ActionResult Edit(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (account.role.title != "Disabled")
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
                return View(patient);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "patientId,medicalRecordNumber,firstName,middleName,lastName,suffix,maidenName,gender,residenceStreetAddress,residenceAptNo,residenceCity,residenceState,residenceZip,mailingStreetAddress,mailingAptNo,mailingCity,mailingState,mailingZip,SSN,motherSSN,fatherSSN,birthDate,birthTime,educationEarned,hispanic,height,weight,isMarried,birthState,birthCity,birthFacility,priorFirstName,priorMiddleName,priorLastName,priorSuffix,inCity,dateCreated,dateUpdated,editedBy")] zz_patient patient)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                logger.Info("User " + account.firstName + " " + account.lastName + " edited patient: " + patient.firstName + " " + patient.lastName);
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: patients/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (Request.Cookies["role"].Value != "Data Entry")
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        patient patient = db.patients.Find(id);
        //        if (patient == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(patient);
        //    }
        //    else
        //    {
        //        return RedirectToAction("tempError", "Home");
        //    }
        //}

        //// POST: patients/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    if (Request.Cookies["role"].Value != "Data Entry")
        //    {
        //        patient patient = db.patients.Find(id);
        //        db.patients.Remove(patient);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return RedirectToAction("tempError", "Home");
        //    }
        //}

        public ActionResult SouvenirForm(int? id)
        {
            user account = db.users.Find(UserAccount.GetUserID());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            zz_patient patient = db.zz_patient.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            List<zz_record> records = db.zz_record.Where(r => r.patientId == patient.patientId).ToList();
            ViewBag.records = records;
            //records[0].birthRecord.dateCreated.ToString();
            DateTime time = (DateTime)patient.birthDate;
            int weight = 0;
            if (patient.birthWeight != null)
            {
                weight = (Int32)patient.birthWeight;
            }
            double lbs = weight / 453;
            double remainder = weight % 453;
            double oz = Math.Round(remainder / 28);
            ViewBag.weight = lbs + " pounds " + oz + " ounces";

            string date = time.ToString("M-d-yyyy");
            ViewBag.birthDate = date;

            logger.Info("User " + account.firstName + " " + account.lastName + " generated souviner form for " + patient.firstName + " " + patient.lastName);
            return View(patient);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //-------------PATIENT LOOKUP----------

        public ActionResult PatientLookup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PatientLookup(System.Web.Mvc.FormCollection form)
        {
            ViewBag.statusMessage = "";
            using (HITProjectData_Fall17Entities1 newDB = new HITProjectData_Fall17Entities1())
            {
                List<patient_general_info> patientToCollection = new List<patient_general_info>();
                patient_general_info patients = null;

                string button = Request.Form["button"];


                string mrn = Request.Form["mrn"];
                string ssn = Request.Form["ssn1"];
                string last = Request.Form["last"];
                ViewBag.patient = ssn;
                ViewBag.mrn = mrn;
                if (mrn.Length < 1 && ssn.Length < 1)
                {
                    if (last.Length < 1)
                    {
                        ViewBag.statusMessage = "Both fields were blank when the lookup was submitted";
                    }
                    else {
                        patientToCollection = db.patient_general_info.Where(r => r.last_name.Contains(last)).ToList();
                        return View("PatientResults", patientToCollection);
                    }
                }
                else if (mrn.Length > 0 && ssn.Length > 0)
                {

                    ViewBag.statusMessage = "Data was contained in both input fields. Please look up a patient by MRN or SSN, not both.";
                }
                else if (mrn.Length > 5 && mrn.Length < 7)
                {
                    patients = db.patient_general_info.Where(r => r.medical_record_number == mrn).FirstOrDefault();

                    if (patients != null)
                    {

                        if (patients.birth_date != null && patients.first_name != null)
                        {
                            patientToCollection.Add(patients);
                            return View("PatientResults", patientToCollection);
                        }
                        else {

                            ViewBag.patient = ssn;
                            return View("Create");

                        }

                    }
                    else {

                        ViewBag.patientFound = false;
                        return View();
                    }

                }
                else if (ssn.Length > 8 && ssn.Length < 10)
                {

                    patients = db.patient_general_info.Where(r => r.social_security_number == ssn).FirstOrDefault();

                    if (patients != null)
                    {

                        if (patients.birth_date != null && patients.first_name != null)
                        {
                            patientToCollection.Add(patients);
                            ViewBag.patient = ssn;
                            return View("PatientResults", patientToCollection);
                        }
                        else {
                            ViewBag.patient = ssn;
                            return View("Create");
                        }
                    }
                    else {

                        patient p = db.patients.Where(r => r.social_security_number == ssn).FirstOrDefault();

                        if (p != null)
                        {

                            ViewBag.patient = p.social_security_number;
                            ViewBag.mrn = p.medical_record_number;
                            return View("Create");


                        }
                        else {
                            ViewBag.patientFound = false;
                            return View();
                        }



                    }

                }
                else { ViewBag.statusMessage = "Please make sure that the input is the proper length. MRN(6) SSN(9)"; }

                //Input not filled in.
                return View();
            }
        }

        //Results of Patient Lookup
        public ActionResult PatientResults(IEnumerable<patient_general_info> patients)
        {
            return View(patients);
        }
    }
}
