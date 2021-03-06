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
    
    public partial class patient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public patient()
        {
            this.patient_address = new HashSet<patient_address>();
            this.patient_birth_information = new HashSet<patient_birth_information>();
            this.patient_family = new HashSet<patient_family>();
            this.patient_insurance = new HashSet<patient_insurance>();
            this.patient_name = new HashSet<patient_name>();
        }
    
        public int patient_id { get; set; }
        public string medical_record_number { get; set; }
        public string mother_maiden_name { get; set; }
        public Nullable<int> gender_id { get; set; }
        public string social_security_number { get; set; }
        public Nullable<int> patient_race_id { get; set; }
        public Nullable<int> patient_ethnicity_id { get; set; }
        public Nullable<int> marital_status_id { get; set; }
        public Nullable<bool> in_city { get; set; }
        public System.DateTime date_created { get; set; }
        public Nullable<System.DateTime> date_edited { get; set; }
        public Nullable<int> edited_by { get; set; }
        public string allergies { get; set; }
    
        public virtual aadmin_data_marital_status aadmin_data_marital_status { get; set; }
        public virtual aadmin_data_patient_ethnicity aadmin_data_patient_ethnicity { get; set; }
        public virtual aadmin_data_patient_gender aadmin_data_patient_gender { get; set; }
        public virtual aadmin_data_patient_race aadmin_data_patient_race { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient_address> patient_address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient_birth_information> patient_birth_information { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient_family> patient_family { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient_insurance> patient_insurance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient_name> patient_name { get; set; }
    }
}
