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
    
    public partial class patient_insurance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public patient_insurance()
        {
            this.encounters = new HashSet<encounter>();
        }
    
        public int patient_insurance_id { get; set; }
        public int patient_id { get; set; }
        public int insurance_id { get; set; }
        public string individual_insurance_id { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
        public Nullable<System.DateTime> date_edited { get; set; }
        public Nullable<int> edited_by { get; set; }
    
        public virtual aadmin_data_insurance aadmin_data_insurance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encounter> encounters { get; set; }
        public virtual patient patient { get; set; }
    }
}