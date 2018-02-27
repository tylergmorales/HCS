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
    
    public partial class patient_address
    {
        public int address_id { get; set; }
        public int patient_id { get; set; }
        public string street_address_one { get; set; }
        public string street_address_two { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public Nullable<int> address_type_id { get; set; }
        public System.DateTime date_added { get; set; }
        public Nullable<System.DateTime> date_edited { get; set; }
        public Nullable<int> edited_by { get; set; }
        public Nullable<bool> address_is_valid { get; set; }
    
        public virtual aadmin_data_address_type aadmin_data_address_type { get; set; }
        public virtual patient patient { get; set; }
    }
}
