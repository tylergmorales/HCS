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
    
    public partial class hospital_facility
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public hospital_facility()
        {
            this.encounters = new HashSet<encounter>();
        }
    
        public int facility_id { get; set; }
        public string description { get; set; }
        public System.DateTime date_created { get; set; }
        public Nullable<System.DateTime> date_updated { get; set; }
        public Nullable<int> edited_by { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<encounter> encounters { get; set; }
    }
}
