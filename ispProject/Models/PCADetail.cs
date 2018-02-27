using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    public class PCADetail
    {
        public int pcaId { get; set; }
        public int encounterId { get; set; }
        public string patient_name { get; set; }
        public virtual patient patient { get; set; }
        public string birthdate { get; set; }
        public string mrn { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public decimal temperature { get; set; }
        public Nullable<int> tem_route_type_id { get; set; }
        public string temp_route_type { get; set; }
        public byte pulse { get; set; }
        public Nullable<int> pulse_route_type_id { get; set; }
        public string pulse_route_type { get; set; }
        public byte pulse_oximetry { get; set; }
        public byte respirations { get; set; }
        public string oxygen_flow { get; set; }
        public Nullable<int> o2_dev_id { get; set; }
        public string o2_dev_type { get; set; }
        public byte systolic_b_pressure { get; set; }
        public byte diastolic_b_pressure { get; set; }
        public string pcaComment { get; set; }
        public string wdl { get; set; }
        public Nullable<int> pain_scale_type_id { get; set; }
        public string pain_scale_type { get; set; }
        public byte pain_level_actual { get; set; }
        public byte pain_level__goal { get; set; }
        public string painComment { get; set; }

    }
}