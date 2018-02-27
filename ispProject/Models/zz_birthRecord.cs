//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ispProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class zz_birthRecord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public zz_birthRecord()
        {
            this.zz_record = new HashSet<zz_record>();
        }
    
        public int birthRecordId { get; set; }
        public string certiferName { get; set; }
        public string certiferTitle { get; set; }
        public Nullable<System.DateTime> certiferDate { get; set; }
        public Nullable<System.DateTime> filedDate { get; set; }
        public bool paternityAck { get; set; }
        public bool ssnRequested { get; set; }
        public string facilityId { get; set; }
        public string birthFacility { get; set; }
        public bool homebirth { get; set; }
        public string attendantName { get; set; }
        public string attendantNpi { get; set; }
        public string attendantTitle { get; set; }
        public bool motherTransferred { get; set; }
        public string transferFacility { get; set; }
        public Nullable<System.DateTime> firstPrenatal { get; set; }
        public Nullable<System.DateTime> lastPrenatal { get; set; }
        public string totalPrenatal { get; set; }
        public Nullable<int> motherPreWeight { get; set; }
        public Nullable<int> motherPostWeight { get; set; }
        public Nullable<int> motherDeliveryWeight { get; set; }
        public bool hadWic { get; set; }
        public Nullable<int> previousBirthLiving { get; set; }
        public Nullable<int> previousBirthDead { get; set; }
        public Nullable<System.DateTime> lastLiveBirth { get; set; }
        public Nullable<int> otherBirthOutcomes { get; set; }
        public Nullable<System.DateTime> lastOtherOutcome { get; set; }
        public string cigThreeBefore { get; set; }
        public string packThreeBefore { get; set; }
        public string cigFirstThree { get; set; }
        public string packFirstThree { get; set; }
        public string cigSecondThree { get; set; }
        public string packSecondThree { get; set; }
        public string cigThirdTri { get; set; }
        public string packThirdTri { get; set; }
        public string paymentSource { get; set; }
        public Nullable<System.DateTime> dateLastMenses { get; set; }
        public bool diabetesPrepregnancy { get; set; }
        public bool diabetesGestational { get; set; }
        public bool hyperTensionPrepregnancy { get; set; }
        public bool hyperTensionGestational { get; set; }
        public bool hyperTensionEclampsia { get; set; }
        public bool prePreTerm { get; set; }
        public bool prePoorOutcome { get; set; }
        public bool resultInfertility { get; set; }
        public bool fertilityDrug { get; set; }
        public bool assistedTech { get; set; }
        public bool previousCesarean { get; set; }
        public Nullable<int> previousCesareanAmount { get; set; }
        public bool gonorrhea { get; set; }
        public bool syphilis { get; set; }
        public bool chlamydia { get; set; }
        public bool hepB { get; set; }
        public bool hepC { get; set; }
        public bool cervicalCerclage { get; set; }
        public bool tocolysis { get; set; }
        public bool externalCephalic { get; set; }
        public bool preRuptureMembrane { get; set; }
        public bool preLabor { get; set; }
        public bool proLabor { get; set; }
        public bool inductionLabor { get; set; }
        public bool augmentationLabor { get; set; }
        public bool nonvertex { get; set; }
        public bool steroids { get; set; }
        public bool antibotics { get; set; }
        public bool chorioamnionitis { get; set; }
        public bool meconium { get; set; }
        public bool fetalIntolerance { get; set; }
        public bool epidural { get; set; }
        public bool unsuccessfulForceps { get; set; }
        public bool unsuccessfulVacuum { get; set; }
        public bool cephalic { get; set; }
        public bool breech { get; set; }
        public bool otherFetalPresentation { get; set; }
        public bool finalSpontaneous { get; set; }
        public bool finalForceps { get; set; }
        public bool finalVacuum { get; set; }
        public bool finalCesarean { get; set; }
        public bool finalTrialOfLabor { get; set; }
        public bool maternalTransfusion { get; set; }
        public bool perinealLaceration { get; set; }
        public bool rupturedUterus { get; set; }
        public bool hysterectomy { get; set; }
        public bool admitICU { get; set; }
        public bool unplannedOperating { get; set; }
        public string fiveMinAgpar { get; set; }
        public string tenMinAgpar { get; set; }
        public string plurality { get; set; }
        public string birthOrder { get; set; }
        public bool ventImmedite { get; set; }
        public bool ventSixHours { get; set; }
        public bool nicu { get; set; }
        public bool surfactant { get; set; }
        public bool neoNatalAntibotics { get; set; }
        public bool seizureDysfunction { get; set; }
        public bool birthInjury { get; set; }
        public bool anencephaly { get; set; }
        public bool meningomyelocele { get; set; }
        public bool cyanotic { get; set; }
        public bool cogenital { get; set; }
        public bool omphalocele { get; set; }
        public bool gastroschisis { get; set; }
        public bool limbReduction { get; set; }
        public bool cleftLip { get; set; }
        public bool cleftPalate { get; set; }
        public bool downConfirmed { get; set; }
        public bool downPending { get; set; }
        public bool suspectedConfirmed { get; set; }
        public bool suspectedPending { get; set; }
        public bool hypospadias { get; set; }
        public bool infantTransferred { get; set; }
        public string infantLiving { get; set; }
        public bool breastFed { get; set; }
        public System.DateTime dateCreated { get; set; }
        public System.DateTime dateEdited { get; set; }
        public Nullable<int> editedBy { get; set; }
        public string estimateGestation { get; set; }
        public Nullable<int> birthWeight { get; set; }
        public bool downSyndrome { get; set; }
        public bool suspectedDisorder { get; set; }
        public string infantTransferFacility { get; set; }
        public bool noPrenatal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<zz_record> zz_record { get; set; }
    }
}
