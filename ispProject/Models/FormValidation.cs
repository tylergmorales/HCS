using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    [MetadataType(typeof(BirthRecordFormValidation))]
    public partial class birthRecord
    {
        // Note this class has nothing in it.  It's just here to add the class-level attribute.
    }



    public class BirthRecordFormValidation
    {
        // Name the field the same as EF named the property - "FirstName" for example.
        // Also, the type needs to match.  Basically just redeclare it.
        // Note that this is a field.  I think it can be a property too, but fields definitely should work.

        [Display(Name = "Birth Record ID")]
        public int birthRecordId { get; set; }
        [Display(Name = "Certifier's Name")]
        public string certiferName { get; set; }
        [Display(Name = "Certifier's Title")]
        public string certiferTitle { get; set; }
        [Display(Name = "Date Certified")]
        public Nullable<System.DateTime> certiferDate { get; set; }
        [Display(Name = "Date Filed")]
        public Nullable<System.DateTime> filedDate { get; set; }
        [Display(Name = "Paternaty Acknowledgement?")]
        public bool paternityAck { get; set; }
        [Display(Name = "SSN Requested?")]
        public bool ssnRequested { get; set; }
        [Display(Name = "Facility ID")]
        public string facilityId { get; set; }
        [Display(Name = "Facility of Birth")]
        public string birthFacility { get; set; }
        [Display(Name = "Home Birth?")]
        public bool homebirth { get; set; }
        [Display(Name = "Attendant's Name")]
        public string attendantName { get; set; }
        [Display(Name = "Attendant's NPI")]
        public string attendantNpi { get; set; }
        [Display(Name = "Attendant's Title")]
        public string attendantTitle { get; set; }
        [Display(Name = "Mother Transferred For Maternal, Medical or Fetal Indications For Delivery?")]
        public bool motherTransferred { get; set; }
        [Display(Name = "Enter Name of Facility")]
        public string transferFacility { get; set; }
        [Display(Name = "Date of First Prenatal Care Visit")]
        public Nullable<System.DateTime> firstPrenatal { get; set; }
        [Display(Name = "Date of Last Prenatal Care Visit")]
        public Nullable<System.DateTime> lastPrenatal { get; set; }
        [Display(Name = "Total Number of Prenatal Visits This Pregnancy")]
        public string totalPrenatal { get; set; }
        [Display(Name = "Prepregnancy Weight")]
        public Nullable<int> motherPreWeight { get; set; }
        [Display(Name = "Weight After Delivery")]
        public Nullable<int> motherPostWeight { get; set; }
        [Display(Name = "Weight At Delivery")]
        public Nullable<int> motherDeliveryWeight { get; set; }
        [Display(Name = "Had WIC Food During Pregnancy?")]
        public bool hadWic { get; set; }
        [Display(Name = "Number of Previous Living Births")]
        public Nullable<int> previousBirthLiving { get; set; }
        [Display(Name = "Number of Previous Dead Births")]
        public Nullable<int> previousBirthDead { get; set; }
        [Display(Name = "Date of Last Live Birth")]
        public Nullable<System.DateTime> lastLiveBirth { get; set; }
        [Display(Name = "Number of Births With Other Outcomes")]
        public Nullable<int> otherBirthOutcomes { get; set; }
        [Display(Name = "Date of Last Birth With Other Outcome")]
        public Nullable<System.DateTime> lastOtherOutcome { get; set; }
        [Display(Name = "Number of Cigarettes Three Months Before Pregnancy")]
        public string cigThreeBefore { get; set; }
        [Display(Name = "Number of Packs Three Months Before Pregnancy")]
        public string packThreeBefore { get; set; }
        [Display(Name = "Number of Cigarettes In The First Trimester of Pregnancy")]
        public string cigFirstThree { get; set; }
        [Display(Name = "Number of Packs In The First Trimester of Pregnancy")]
        public string packFirstThree { get; set; }
        [Display(Name = "Number of Cigarettes In The Second Trimester of Pregnancy")]
        public string cigSecondThree { get; set; }
        [Display(Name = "Number of Packs In The Second Trimester of Pregnancy ")]
        public string packSecondThree { get; set; }
        [Display(Name = "Number of Cigarettes In The Third Trimester of Pregnancy")]
        public string cigThirdTri { get; set; }
        [Display(Name = "Number of Packs In The Third Trimester of Pregnancy")]
        public string packThirdTri { get; set; }
        [Display(Name = "Source of Payment For This Delivery")]
        public string paymentSource { get; set; }
        [Display(Name = "Date Last Normal Menses Began")]
        public Nullable<System.DateTime> dateLastMenses { get; set; }
        [Display(Name = "Prepregnancy Diabetes?")]
        public bool diabetesPrepregnancy { get; set; }
        [Display(Name = "Gestational Diabetes?")]
        public bool diabetesGestational { get; set; }
        [Display(Name = "Prepregnancy Hypertension?")]
        public bool hyperTensionPrepregnancy { get; set; }
        [Display(Name = "Gestational Hypertension?")]
        public bool hyperTensionGestational { get; set; }
        [Display(Name = "Eclampsia Hypertension?")]
        public bool hyperTensionEclampsia { get; set; }
        [Display(Name = "Previous Preterm Birth?")]
        public bool prePreTerm { get; set; }
        [Display(Name = "Other Previous Poor Pregnancy Outcome?")]
        public bool prePoorOutcome { get; set; }
        [Display(Name = "Pregnancy Resulted From Infertility Treatment?")]
        public bool resultInfertility { get; set; }
        [Display(Name = "Pregnancy Resulted From Fertility-Enhancing Drugs, Artificial Insemination or Intrauterine Insemination?")]
        public bool fertilityDrug { get; set; }
        [Display(Name = "Pregnancy Resulted From Assisted Reproductive Technology?")]
        public bool assistedTech { get; set; }
        [Display(Name = "Had A Previous Cesarean Delivery?")]
        public bool previousCesarean { get; set; }
        [Display(Name = "Number of Previous Cesareans")]
        public Nullable<int> previousCesareanAmount { get; set; }
        [Display(Name = "Gonorrhea?")]
        public bool gonorrhea { get; set; }
        [Display(Name = "Syphilis?")]
        public bool syphilis { get; set; }
        [Display(Name = "Chlamydia?")]
        public bool chlamydia { get; set; }
        [Display(Name = "Hepatitis B?")]
        public bool hepB { get; set; }
        [Display(Name = "Hepatitis A?")]
        public bool hepC { get; set; }
        [Display(Name = "Cervical Cerclage?")]
        public bool cervicalCerclage { get; set; }
        [Display(Name = "Tocolysis")]
        public bool tocolysis { get; set; }
        [Display(Name = "External Cephalic Succeeded?")]
        public bool externalCephalic { get; set; }
        [Display(Name = "Premature Rupture of the Membranes?")]
        public bool preRuptureMembrane { get; set; }
        [Display(Name = "Precipitous Labor (<3 hrs.)?")]
        public bool preLabor { get; set; }
        [Display(Name = "Prolonged Labor (>20 hrs.)?")]
        public bool proLabor { get; set; }
        [Display(Name = "Induction of Labor?")]
        public bool inductionLabor { get; set; }
        [Display(Name = "Augmentation of Labor?")]
        public bool augmentationLabor { get; set; }
        [Display(Name = "Non-vertex Presentation?")]
        public bool nonvertex { get; set; }
        [Display(Name = "Steriods (Glucocorticoids) for Fetal Lung Maturation?")]
        public bool steroids { get; set; }
        [Display(Name = "Antibiotics Received During Labor?")]
        public bool antibotics { get; set; }
        [Display(Name = "Clinical Chorioamnionitis Diagnosed During Labor or Maternal Temperature >38°C(100.4°F)?")]
        public bool chorioamnionitis { get; set; }
        [Display(Name = "Mederate/Heavy Meconium Staining of the Amniotic Fluid?")]
        public bool meconium { get; set; }
        [Display(Name = "Fetal Intolerance of Labor?")]
        public bool fetalIntolerance { get; set; }
        [Display(Name = "Epidural or Spinal Anesthesia During Labor?")]
        public bool epidural { get; set; }
        [Display(Name = "Delivery With Forceps Attempted But Unsuccessful?")]
        public bool unsuccessfulForceps { get; set; }
        [Display(Name = "Delivery With Vacuum Extraction Attempted But Unsuccessful?")]
        public bool unsuccessfulVacuum { get; set; }
        [Display(Name = "Cephalic")]
        public bool cephalic { get; set; }
        [Display(Name = "Breech")]
        public bool breech { get; set; }
        [Display(Name = "Other")]
        public bool otherFetalPresentation { get; set; }
        [Display(Name = "Vaginal/Spontaneous")]
        public bool finalSpontaneous { get; set; }
        [Display(Name = "Vaginal/Forceps")]
        public bool finalForceps { get; set; }
        [Display(Name = "Vaginal/Vacuum")]
        public bool finalVacuum { get; set; }
        [Display(Name = "Cesarean")]
        public bool finalCesarean { get; set; }
        [Display(Name = "Was A Trial of Labor Attempted?")]
        public bool finalTrialOfLabor { get; set; }
        [Display(Name = "Maternal Transfusion")]
        public bool maternalTransfusion { get; set; }
        [Display(Name = "Third or Fourth Degree Perineal Laceration?")]
        public bool perinealLaceration { get; set; }
        [Display(Name = "Ruptured Uterus?")]
        public bool rupturedUterus { get; set; }
        [Display(Name = "Unplanned Hysterectomy?")]
        public bool hysterectomy { get; set; }
        [Display(Name = "Admission to Intensive Care Unit?")]
        public bool admitICU { get; set; }
        [Display(Name = "Unplanned Operating Rool Procedure Following Delivery?")]
        public bool unplannedOperating { get; set; }
        [Display(Name = "Score at 5 Minutes")]
        public string fiveMinAgpar { get; set; }
        [Display(Name = "Score at 10 Minutes")]
        public string tenMinAgpar { get; set; }
        [Display(Name = "Plurality (Single, Twin, Triplet, etc.)")]
        public string plurality { get; set; }
        [Display(Name = "Birth Order (First, Second, Third, etc.)")]
        public string birthOrder { get; set; }
        [Display(Name = "Assisted Ventilation Required Immediately Following Deliver?")]
        public bool ventImmedite { get; set; }
        [Display(Name = "Assisted Ventiliation Required For More Than Six Hours?")]
        public bool ventSixHours { get; set; }
        [Display(Name = "NICU Admission?")]
        public bool nicu { get; set; }
        [Display(Name = "Newborn Given Surfactant Replacement Therapy?")]
        public bool surfactant { get; set; }
        [Display(Name = "Antibiotics Received By The Newborn For Suspected Neonatal Septis?")]
        public bool neoNatalAntibotics { get; set; }
        [Display(Name = "Seizure or Serious Neurologic Dysfunction?")]
        public bool seizureDysfunction { get; set; }
        [Display(Name = "Significant Birth Injury?")]
        public bool birthInjury { get; set; }
        [Display(Name = "Anencephaly")]
        public bool anencephaly { get; set; }
        [Display(Name = "Meningomyelocele/Spina Bifida")]
        public bool meningomyelocele { get; set; }
        [Display(Name = "Cyanotic Congenital Hear Disease")]
        public bool cyanotic { get; set; }
        [Display(Name = "Cogenital Diaphragmatic Hernia")]
        public bool cogenital { get; set; }
        [Display(Name = "Omphalocele")]
        public bool omphalocele { get; set; }
        [Display(Name = "Gastroschisis")]
        public bool gastroschisis { get; set; }
        [Display(Name = "Limb Reduction Defect")]
        public bool limbReduction { get; set; }
        [Display(Name = "Cleft Lip")]
        public bool cleftLip { get; set; }
        [Display(Name = "Cleft Palate Alone")]
        public bool cleftPalate { get; set; }
        [Display(Name = "Karyotype Confirmed")]
        public bool downConfirmed { get; set; }
        [Display(Name = "Karyotype Pending")]
        public bool downPending { get; set; }
        [Display(Name = "Karyotype Confirmed")]
        public bool suspectedConfirmed { get; set; }
        [Display(Name = "Karyotype Pending")]
        public bool suspectedPending { get; set; }
        [Display(Name = "Hypospadias")]
        public bool hypospadias { get; set; }
        [Display(Name = "Was Infant Transferred Within 24 Hours of Delivery?")]
        public bool infantTransferred { get; set; }
        [Display(Name = "Is Infant Living At Time of Report?")]
        public string infantLiving { get; set; }
        [Display(Name = "Is the Infanct Being Breastfed At Discharge?")]
        public bool breastFed { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime dateCreated { get; set; }
        [Display(Name = "Date Edited")]
        public System.DateTime dateEdited { get; set; }
        [Display(Name = "Edited By")]
        public Nullable<int> editedBy { get; set; }
        [Display(Name = "Obstetric Estimate of Gestation")]
        public string estimateGestation { get; set; }
        [Display(Name = "Birth Weight")]
        public Nullable<int> birthWeight { get; set; }
        [Display(Name = "Down Syndrome")]
        public bool downSyndrome { get; set; }
        [Display(Name = "Suspected Chromosomal Disorder")]
        public bool suspectedDisorder { get; set; }
        [Display(Name = "Name of Facility Infant Transferred To")]
        public string infantTransferFacility { get; set; }
        [Display(Name = "No Prenatal Visits")]
        public bool noPrenatal { get; set; }
    }

    public class PatientFormValidation
    {
        [Display(Name="ID")]
        public int patientId { get; set; }
        [Display(Name = "Medical Record Number")]
        public string medicalRecordNumber { get; set; }
        [Display(Name = "Current Legal First Name")]
        public string firstName { get; set; }
        [Display(Name = "Current Legal Middle Name")]
        public string middleName { get; set; }
        [Display(Name = "Current Legal Last Name")]
        public string lastName { get; set; }
        [Display(Name = "Suffix")]
        public string suffix { get; set; }
        [Display(Name = "Maiden Name")]
        public string maidenName { get; set; }
        [Display(Name = "Gender")]
        public string gender { get; set; }
        [Display(Name = "Residence Street Address")]
        public string residenceStreetAddress { get; set; }
        [Display(Name = "Residence Apt. No.")]
        public string residenceAptNo { get; set; }
        [Display(Name = "Residence City")]
        public string residenceCity { get; set; }
        [Display(Name = "Residence State")]
        public string residenceState { get; set; }
        [Display(Name = "Residence Zip")]
        public string residenceZip { get; set; }
        [Display(Name = "Mailing Street Address")]
        public string mailingStreetAddress { get; set; }
        [Display(Name = "Mailing Apt. No.")]
        public string mailingAptNo { get; set; }
        [Display(Name = "Mailing City")]
        public string mailingCity { get; set; }
        [Display(Name = "Mailing State")]
        public string mailingState { get; set; }
        [Display(Name = "Mailing Zip")]
        public string mailingZip { get; set; }
        [Display(Name = "SSN")]
        public string SSN { get; set; }
        [Display(Name = "Mother's SSN")]
        public string motherSSN { get; set; }
        [Display(Name = "Father's SSN")]
        public string fatherSSN { get; set; }
        [Display(Name = "Birth Date")]
        public Nullable<System.DateTime> birthDate { get; set; }
        [Display(Name = "Birth Time")]
        public Nullable<System.TimeSpan> birthTime { get; set; }
        [Display(Name = "Highest Edication Earned")]
        public string educationEarned { get; set; }
        [Display(Name = "Hispanic Origin")]
        public string hispanic { get; set; }
        [Display(Name = "Height")]
        public Nullable<int> height { get; set; }
        [Display(Name = "Weight")]
        public Nullable<int> weight { get; set; }
        [Display(Name = "Married?")]
        public bool isMarried { get; set; }
        [Display(Name = "State of Birth")]
        public string birthState { get; set; }
        [Display(Name = "City of Birth")]
        public string birthCity { get; set; }
        [Display(Name = "Facility of Birth")]
        public string birthFacility { get; set; }
        [Display(Name = "Prior First Name")]
        public string priorFirstName { get; set; }
        [Display(Name = "Prior Middle Name")]
        public string priorMiddleName { get; set; }
        [Display(Name = "Prior Last Name")]
        public string priorLastName { get; set; }
        [Display(Name = "Prior Suffix")]
        public string priorSuffix { get; set; }
        [Display(Name = "Within city limits?")]
        public bool inCity { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime dateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime dateUpdated { get; set; }
        [Display(Name = "Edited By")]
        public Nullable<int> editedBy { get; set; }
        [Display(Name = "County of Birth")]
        public string birthCounty { get; set; }
        [Display(Name = "County of Residence")]
        public string residenceCounty { get; set; }
        [Display(Name = "Weight at Birth")]
        public Nullable<int> birthWeight { get; set; }
    }
}