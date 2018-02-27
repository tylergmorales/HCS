using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ispProject.Models
{
    public class UpdateVitals
    {

        public int pcaId { get; set; }
        public int encounterId { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public decimal temperature { get; set; }
        public int temp_route_value { get; set; }
        public string temp_route_type { get; set; }
        public byte pulse { get; set; }
        public int pulse_route_value { get; set; }
        public string pulse_route_type { get; set; }
        public byte pulse_oximetry { get; set; }
        public byte respirations { get; set; }
        public string oxygen_flow { get; set; }
        public int o2_dev_method { get; set; }
        public string o2_type { get; set; }
        public byte systolic_b_pressure { get; set; }
        public byte diastolic_b_pressure { get; set; }
        public string comment { get; set; }
        public string wdl_ex { get; set; }
        public int pain_scale_value { get; set; }
        public string pain_scale_type { get; set; }
        public virtual nursing_pain_scale_type selectlist { get; set; }
        public byte pain_level_actual { get; set; }
        public byte pain_level__goal { get; set; }
        public string care_comment { get; set; }
       
    }
}