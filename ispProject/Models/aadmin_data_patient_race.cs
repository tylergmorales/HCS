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
    
    public partial class aadmin_data_patient_race
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public aadmin_data_patient_race()
        {
            this.patients = new HashSet<patient>();
        }
    
        public int patient_race_id { get; set; }
        public string description { get; set; }
        public System.DateTime date_added { get; set; }
        public Nullable<System.DateTime> date_edited { get; set; }
        public Nullable<int> edited_by { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<patient> patients { get; set; }
    }
}
