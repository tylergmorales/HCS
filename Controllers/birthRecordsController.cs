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

namespace ispProject.Controllers
{
    [Authorize]
    public class birthRecordsController : Controller
    {
        private HITProjectData_Fall17Entities1 newDB = new HITProjectData_Fall17Entities1();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: birthRecords
        public ActionResult Index() 
        {
            return View(newDB.zz_birthRecord.ToList());
        }

        public ActionResult formPdf(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("tempError", "Home");
            }
            BirthRecordFormModel brfm = new BirthRecordFormModel();
            brfm.birthRecord = newDB.zz_birthRecord.Find(id);
            brfm.motherPatient = newDB.zz_patient.Find(newDB.zz_record.Where(r => r.birthRecordId == id).FirstOrDefault());
            if (newDB.zz_record.Where(r => r.birthRecordId == id).Count() > 2)
            {
                brfm.fatherPatient = newDB.zz_patient.Find(newDB.zz_record.Where(r => r.birthRecordId == id).ElementAtOrDefault(1));
                brfm.childPatient = newDB.zz_patient.Find(newDB.zz_record.Where(r => r.birthRecordId == id).ElementAtOrDefault(2));
            }
            else
                brfm.childPatient = newDB.zz_patient.Find(newDB.zz_record.Where(r => r.birthRecordId == id).ElementAtOrDefault(1));

            return View("formPdf", brfm);
        }

        // GET: birthRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            zz_birthRecord birthRecord = newDB.zz_birthRecord.Find(id);
            if (birthRecord == null)
            {
                return HttpNotFound();
            }
            return View(birthRecord);
        }

        //// GET: birthRecords/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        public ActionResult Create(int? id)
        {
            user account = newDB.users.Find(UserAccount.GetUserID());
            if (account.role.title != "Disabled")
            {
                ViewBag.patientId = id;
                zz_patient patient = newDB.zz_patient.Find(id);
                if (patient == null)
                {
                    ViewBag.patientFound = false;
                    RedirectToAction("PatientLookup", "patients");
                }
                //Patient Not Null
                ViewBag.motherFirstName = patient.firstName;
                ViewBag.motherMiddleName = patient.middleName;
                ViewBag.motherLastName = patient.lastName;
                ViewBag.motherSuffix = patient.suffix;
                DateTime motherDOB = patient.birthDate.Value;
                ViewBag.motherDOB = motherDOB.ToString("yyyy-MM-dd");
                ViewBag.motherBirthplace = patient.birthState;
                DateTime todayDate = DateTime.Now;
                ViewBag.childDOB = todayDate.ToString("yyyy-MM-dd");
                TimeSpan time = TimeSpan.Zero;
                ViewBag.childTime = time;
                return View();
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: birthRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "birthRecordId,certiferName,certiferTitle,certiferDate,filedDate,paternityAck,ssnRequested,facilityId,birthFacility,homebirth,attendantName,attendantNpi,attendantTitle,motherTransferred,transferFacility,firstPrenatal,lastPrenatal,totalPrenatal,motherPreWeight,motherPostWeight,motherDeliveryWeight,hadWic,previousBirthLiving,previousBirthDead,lastLiveBirth,otherBirthOutcomes,lastOtherOutcome,cigThreeBefore,packThreeBefore,cigFirstThree,packFirstThree,cigSecondThree,packSecondThree,cigThirdTri,packThirdTri,paymentSource,dateLastMenses,diabetesPrepregnancy,diabetesGestational,hyperTensionPrepregnancy,hyperTensionGestational,hyperTensionEclampsia,prePreTerm,prePoorOutcome,resultInfertility,fertilityDrug,assistedTech,previousCesarean,previousCesareanAmount,gonorrhea,syphilis,chlamydia,hepB,hepC,cervicalCerclage,tocolysis,externalCephalic,preRuptureMembrane,preLabor,proLabor,inductionLabor,augmentationLabor,nonvertex,steroids,antibotics,chorioamnionitis,meconium,fetalIntolerance,epidural,unsuccessfulForceps,unsuccessfulVacuum,cephalic,breech,otherFetalPresentation,finalSpontaneous,finalForceps,finalVacuum,finalCesarean,finalTrialOfLabor,maternalTransfusion,perinealLaceration,rupturedUterus,hysterectomy,admitICU,unplannedOperating, birthWeight, fiveMinAgpar,tenMinAgpar,plurality,birthOrder,ventImmedite,ventSixHours,nicu,surfactant,neoNatalAntibotics,seizureDysfunction,birthInjury,anencephaly,meningomyelocele,cyanotic,cogenital,omphalocele,gastroschisis,limbReduction,cleftLip,cleftPalate,downConfirmed,downPending,suspectedConfirmed,suspectedPending,hypospadias,infantTransferred,infantLiving,breastFed,dateCreated,dateEdited,editedBy")] zz_birthRecord birthRecord, FormCollection form)
        {
            user account = newDB.users.Find(UserAccount.GetUserID());
            if (ModelState.IsValid)
            {
                //Create Child Info
                zz_patient child = new zz_patient();
                child.firstName = form["childFirstName"];
                child.middleName = form["childMiddleName"];
                child.lastName = form["childLastName"];
                child.suffix = form["childSuffix"];
                child.birthTime = TimeSpan.Parse(form["timeOfBirth"]);
                child.gender = form["genderList"];
                child.birthDate = Convert.ToDateTime(form["childBirthDate"]);
                child.birthFacility = form["facilityName"];
                child.birthCity = form["childBirthLocation"];
                child.birthCounty = form["childBirthCounty"];
                child.birthWeight = Convert.ToInt32(form["birthWeight"]);
                if (Request.Cookies["userId"] != null)
                {
                    HttpCookie aCookie = Request.Cookies["userId"];
                    child.editedBy = Convert.ToInt32(Server.HtmlEncode(aCookie.Value));
                }
                child.dateCreated = DateTime.Now;
                child.dateUpdated = DateTime.Now;

                Random random = new Random();
                int childMRN = random.Next(100000000, 999999999);

                using (HITProjectData_Fall17Entities1 newDB = new HITProjectData_Fall17Entities1())
                {
                    string randomNumberString = Convert.ToString(childMRN);
                    List<zz_patient> patients = newDB.zz_patient.Where(p => p.medicalRecordNumber == randomNumberString).ToList();
                    while (patients.Count > 0)
                    {
                        childMRN = random.Next(100000000, 999999999);
                        randomNumberString = Convert.ToString(childMRN);
                        patients = newDB.zz_patient.Where(p => p.medicalRecordNumber == randomNumberString).ToList();
                    }
                }
                child.medicalRecordNumber = Convert.ToString(childMRN);
                //Add and save child to Database
                newDB.zz_patient.Add(child);
                newDB.SaveChanges();

                //Mother info
                int id = Convert.ToInt32(form["motherId"]);
                zz_patient mother = newDB.zz_patient.Find(id);
                mother.firstName = form["motherFirstName"];
                mother.middleName = form["motherMiddleName"];
                mother.lastName = form["motherLastName"];
                mother.suffix = form["motherSuffix"];
                mother.birthDate = Convert.ToDateTime(form["motherDOB"]);
                mother.priorFirstName = form["motherPriorFirstName"];
                mother.priorMiddleName = form["motherPriorMiddleName"];
                mother.priorLastName = form["motherPriorLastName"];
                mother.priorSuffix = form["motherPriorSuffix"];
                mother.residenceState = form["state"];
                mother.residenceCounty = form["motherCountry"];
                mother.residenceCity = form["motherCity"];
                mother.residenceStreetAddress = form["motherAddress"];
                mother.residenceAptNo = form["motherAptNo"];
                mother.residenceZip = form["motherZip"];
                mother.inCity = Convert.ToBoolean(form["inCity"].Split(',')[0]);
                newDB.Entry(mother).State = EntityState.Modified;
                newDB.SaveChanges();

                //Add Mother address to child
                child.residenceState = form["state"];
                child.residenceCounty = form["motherCountry"];
                child.residenceCity = form["motherCity"];
                child.residenceStreetAddress = form["motherAddress"];
                child.residenceAptNo = form["motherAptNo"];
                child.residenceAptNo = form["motherZip"];
                newDB.Entry(child).State = EntityState.Modified;
                newDB.SaveChanges();

                //Father Information
                zz_patient father = new zz_patient();
                //If father information exists
                //TODO add logic for father lookup
                if (form["fatherFirstName"] != "")
                {
                    father.firstName = form["fatherFirstName"];
                    father.middleName = form["fatherMiddleName"];
                    father.lastName = form["fatherLastName"];
                    father.suffix = form["fatherSuffix"];
                    father.birthDate = Convert.ToDateTime(form["fatherDOB"]);
                    father.birthState = form["fatherBirthplace"];
                    int fatherMRN = random.Next(100000000, 999999999);

                    using (HITProjectData_Fall17Entities1 newDB = new HITProjectData_Fall17Entities1())
                    {
                        string randomNumberString = Convert.ToString(fatherMRN);
                        List<zz_patient> patients = newDB.zz_patient.Where(p => p.medicalRecordNumber == randomNumberString).ToList();
                        while (patients.Count > 0)
                        {
                            fatherMRN = random.Next(100000000, 999999999);
                            randomNumberString = Convert.ToString(fatherMRN);
                            patients = newDB.zz_patient.Where(p => p.medicalRecordNumber == randomNumberString).ToList();
                        }
                    }
                    father.medicalRecordNumber = Convert.ToString(fatherMRN);

                    //Add and save father to Database
                    newDB.zz_patient.Add(father);
                    newDB.SaveChanges();
                }

                //Certifier information
                birthRecord.certiferName = form["certifierName"];
                birthRecord.certiferTitle = form["certifierTitleList"];
                if (birthRecord.certiferTitle == "Other")
                {
                    birthRecord.certiferTitle = form["certifierTitleOther"];
                }
                birthRecord.certiferDate = Convert.ToDateTime(form["dateCertified"]);
                birthRecord.filedDate = Convert.ToDateTime(form["datedFiled"]);

                //Mother2 Info
                mother.mailingState = form["stateList"];
                mother.mailingCity = form["motherMailingCity"];
                mother.mailingStreetAddress = form["motherMailingAddress"];
                mother.mailingAptNo = form["motherMailingAptNo"];
                mother.mailingZip = form["motherMailingZip"];
                mother.isMarried = Convert.ToBoolean(form["isMarried"].Split(',')[0]);
                //TODO null check
                mother.SSN = form["motherSSN"];
                child.motherSSN = form["motherSSN"];
                //TODO null check
                if (form["fatherFirstName"] != "")
                {
                    father.SSN = form["fatherSSN"];
                    child.fatherSSN = form["fatherSSN"];
                    newDB.Entry(father).State = EntityState.Modified;
                }
                newDB.Entry(child).State = EntityState.Modified;
                newDB.Entry(mother).State = EntityState.Modified;
                newDB.SaveChanges();

                //Mother3 Info
                mother.educationEarned = form["motherEducation"];
                mother.hispanic = form["motherHispanic"];
                if (mother.hispanic == "Yes, other Spanish/Hispanic/Latina")
                {
                    mother.hispanic = "motherHispanicOther";
                }
                newDB.Entry(mother).State = EntityState.Modified;
                newDB.SaveChanges();

                zz_patientRace motherRace = new zz_patientRace();
                motherRace.patientId = id;
                motherRace.white = Convert.ToBoolean(form["motherWhite"].Split(',')[0]);
                motherRace.black = Convert.ToBoolean(form["motherBlack"].Split(',')[0]);
                motherRace.tribe = form["mothertribe"];
                motherRace.asianIndian = Convert.ToBoolean(form["motherAsianIndian"].Split(',')[0]);
                motherRace.chinese = Convert.ToBoolean(form["motherChinese"].Split(',')[0]);
                motherRace.flipino = Convert.ToBoolean(form["motherFilipino"].Split(',')[0]);
                motherRace.japanese = Convert.ToBoolean(form["motherJapanese"].Split(',')[0]);
                motherRace.korean = Convert.ToBoolean(form["motherKorean"].Split(',')[0]);
                motherRace.vietnamese = Convert.ToBoolean(form["motherVietnamese"].Split(',')[0]);
                motherRace.otherAsian = form["motherOtherAsian"];
                motherRace.hawaiian = Convert.ToBoolean(form["motherHawaiian"].Split(',')[0]);
                motherRace.guamanian = Convert.ToBoolean(form["motherGuamanian"].Split(',')[0]);
                motherRace.samoan = Convert.ToBoolean(form["motherSamoan"].Split(',')[0]);
                motherRace.pacificIslander = form["motherOtherIslander"];
                motherRace.other = form["motherOtherRace"];
                newDB.zz_patientRace.Add(motherRace);
                newDB.SaveChanges();

                //Father 2 Information
                if (form["fatherFirstName"] != "")
                {
                    father.educationEarned = form["fatherEducation"];
                    father.hispanic = form["fatherHispanic"];
                    if (father.hispanic == "Yes, other Spanish/Hispanic/Latina")
                    {
                        father.hispanic = "fatherHispanicOther";
                    }
                    newDB.Entry(father).State = EntityState.Modified;
                    newDB.SaveChanges();

                    List<zz_patient> patients = newDB.zz_patient.Where(p => p.medicalRecordNumber == father.medicalRecordNumber).ToList();
                    zz_patientRace fatherRace = new zz_patientRace();
                    fatherRace.patientId = patients[0].patientId;
                    fatherRace.white = Convert.ToBoolean(form["fatherWhite"].Split(',')[0]);
                    fatherRace.black = Convert.ToBoolean(form["fatherBlack"].Split(',')[0]);
                    fatherRace.tribe = form["fatherTribe"];
                    fatherRace.asianIndian = Convert.ToBoolean(form["fatherAsianIndian"].Split(',')[0]);
                    fatherRace.chinese = Convert.ToBoolean(form["fatherChinese"].Split(',')[0]);
                    fatherRace.flipino = Convert.ToBoolean(form["fatherFilipino"].Split(',')[0]);
                    fatherRace.japanese = Convert.ToBoolean(form["fatherJapanese"].Split(',')[0]);
                    fatherRace.korean = Convert.ToBoolean(form["fatherKorean"].Split(',')[0]);
                    fatherRace.vietnamese = Convert.ToBoolean(form["fatherVietnamese"].Split(',')[0]);
                    fatherRace.otherAsian = form["fatherOtherAsian"];
                    fatherRace.hawaiian = Convert.ToBoolean(form["fatherHawaiian"].Split(',')[0]);
                    fatherRace.guamanian = Convert.ToBoolean(form["fatherGuamanian"].Split(',')[0]);
                    fatherRace.samoan = Convert.ToBoolean(form["fatherSamoan"].Split(',')[0]);
                    fatherRace.pacificIslander = form["fatherOtherIslander"];
                    fatherRace.other = form["fatherOtherRace"];
                    newDB.zz_patientRace.Add(fatherRace);
                    newDB.SaveChanges();
                }

                birthRecord.birthFacility = form["birthOccuredPlace"];
                if (birthRecord.birthFacility == "Other")
                {
                    birthRecord.birthFacility = form["birthOccuredPlaceOther"];
                }

                birthRecord.attendantTitle = form["attendantTitleList"];
                if (birthRecord.attendantTitle == "Other")
                {
                    birthRecord.attendantTitle = form["attendantNPIOther"];
                }
                birthRecord.noPrenatal = Convert.ToBoolean(form["noPrenatal"].Split(',')[0]);
                if (birthRecord.noPrenatal == false)
                {
                    birthRecord.firstPrenatal = Convert.ToDateTime(form["firstPrenatal"]);
                    birthRecord.lastPrenatal = Convert.ToDateTime(form["lastPrenatal"]);
                }
                mother.height = Convert.ToInt32(form["motherHeight"]);
                birthRecord.lastLiveBirth = Convert.ToDateTime(form["lastLiveBirth"]);
                birthRecord.lastOtherOutcome = Convert.ToDateTime(form["lastOtherOutcome"]);
                birthRecord.paymentSource = form["paymentList"];
                if (birthRecord.paymentSource == "Other")
                {
                    birthRecord.paymentSource = form["paymentOther"];
                }
                birthRecord.dateLastMenses = Convert.ToDateTime(form["dateLastMenses"]);
                birthRecord.infantLiving = form["infantLiving"];

                newDB.Entry(mother).State = EntityState.Modified;
                newDB.zz_birthRecord.Add(birthRecord);
                newDB.SaveChanges();

                //TODO add record to bridging table
                //Add Mother
                zz_record motherR = new zz_record();
                var userId = Convert.ToInt32(Request.Cookies["userId"].Value);
                List<user> users = newDB.users.Where(u => u.userId == userId).ToList();
                motherR.hospitalId = users[0].hospitalId;
                motherR.patientId = mother.patientId;
                motherR.birthRecordId = birthRecord.birthRecordId;
                newDB.zz_record.Add(motherR);
                newDB.SaveChanges();

                //Add Father
                if (form["fatherFirstName"] != "")
                {
                    zz_record fatherR = new zz_record();
                    fatherR.hospitalId = users[0].hospitalId;
                    fatherR.patientId = father.patientId;
                    fatherR.birthRecordId = birthRecord.birthRecordId;
                    newDB.zz_record.Add(fatherR);
                    newDB.SaveChanges();
                }

                //Add Child
                zz_record childR = new zz_record();
                childR.hospitalId = users[0].hospitalId;
                childR.patientId = child.patientId;
                childR.birthRecordId = birthRecord.birthRecordId;
                newDB.zz_record.Add(childR);
                newDB.SaveChanges();
                logger.Info("User " + account.firstName + " " + account.lastName + " created birth record: " + birthRecord.birthRecordId);

                return RedirectToAction("Index");
            }

            return View(birthRecord);
        }

        // GET: birthRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            user account = newDB.users.Find(UserAccount.GetUserID());
            if (account.role.title != "Disabled")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                zz_birthRecord birthRecord = newDB.zz_birthRecord.Find(id);
                if (birthRecord == null)
                {
                    return HttpNotFound();
                }
                return View(birthRecord);
            }
            else
            {
                return RedirectToAction("tempError", "Home");
            }
        }

        // POST: birthRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "birthRecordId,certiferName,certiferTitle,certiferDate,filedDate,paternityAck,ssnRequested,facilityId,birthFacility,homebirth,attendantName,attendantNpi,attendantTitle,motherTransferred,transferFacility,firstPrenatal,lastPrenatal,totalPrenatal,motherPreWeight,motherPostWeight,motherDeliveryWeight,hadWic,previousBirthLiving,previousBirthDead,lastLiveBirth,otherBirthOutcomes,lastOtherOutcome,cigThreeBefore,packThreeBefore,cigFirstThree,packFirstThree,cigSecondThree,packSecondThree,cigThirdTri,packThirdTri,paymentSource,dateLastMenses,diabetesPrepregnancy,diabetesGestational,hyperTensionPrepregnancy,hyperTensionGestational,hyperTensionEclampsia,prePreTerm,prePoorOutcome,resultInfertility,fertilityDrug,assistedTech,previousCesarean,previousCesareanAmount,gonorrhea,syphilis,chlamydia,hepB,hepC,cervicalCerclage,tocolysis,externalCephalic,preRuptureMembrane,preLabor,proLabor,inductionLabor,augmentationLabor,nonvertex,steroids,antibotics,chorioamnionitis,meconium,fetalIntolerance,epidural,unsuccessfulForceps,unsuccessfulVacuum,cephalic,breech,otherFetalPresentation,finalSpontaneous,finalForceps,finalVacuum,finalCesarean,finalTrialOfLabor,maternalTransfusion,perinealLaceration,rupturedUterus,hysterectomy,admitICU,unplannedOperating,fiveMinAgpar,tenMinAgpar,plurality,birthOrder,ventImmedite,ventSixHours,nicu,surfactant,neoNatalAntibotics,seizureDysfunction,birthInjury,anencephaly,meningomyelocele,cyanotic,cogenital,omphalocele,gastroschisis,limbReduction,cleftLip,cleftPalate,downConfirmed,downPending,suspectedConfirmed,suspectedPending,hypospadias,infantTransferred,infantLiving,breastFed,dateCreated,dateEdited,editedBy")] zz_birthRecord birthRecord)
        {
            user account = newDB.users.Find(UserAccount.GetUserID());
            if (ModelState.IsValid)
            {
                newDB.Entry(birthRecord).State = EntityState.Modified;
                newDB.SaveChanges();
                logger.Info("User " + account.firstName + " " + account.lastName + " edited birth record: " + birthRecord.birthRecordId);
                return RedirectToAction("Index");
            }
            return View(birthRecord);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                newDB.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
