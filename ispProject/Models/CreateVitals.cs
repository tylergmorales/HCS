using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ispProject.Models
{

    public class CreateVitals
    {
        [Display(Name = "PCA ID")]
        public int pcaId { get; set; }
        [Display(Name = "Encounterid")]
        public int encounterId { get; set; }
        [Required]
        [DisplayName("DateAdded")]
        public Nullable<System.DateTime> datetime { get; set; }
        [DisplayName("Temperature")]
        public decimal temperature { get; set; }
        [DisplayName("Temp_Route")]
        public int temp_route_value { get; set; }
        [Required]
        [DisplayName("Pulse")]
        public byte pulse { get; set; }
        [Required]
        [DisplayName("Pulse_Route")]
        public int pulse_route_value { get; set; }
        [DisplayName("Pulse Oximetry")]
        public byte pulse_oximetry { get; set; }
        [DisplayName("Respirations")]
        public byte respirations { get; set; }
        [DisplayName("Oxygen Flow")]
        public string oxygen_flow { get; set; }
        [DisplayName("O2 Delivery Method")]
        public int o2_dev_method { get; set; }
        [Required]
        [DisplayName("Systolic Blood Pressure")]
        public byte systolic_b_pressure { get; set; }
        [Required]
        [DisplayName("Diastolic Blood Pressure")]
        public byte diastolic_b_pressure { get; set; }
        [DisplayName("PCA Comments")]
        public string comment { get; set; }
        [DisplayName("Pain/Comfort/Coping")]
        public Boolean wdl_ex { get; set; }
        public int pain_scale_value { get; set; }
        [DisplayName("Pain Scale")]
        public byte pain_level_actual { get; set; }
        [DisplayName("Pain Level Goal")]
        public byte pain_level__goal { get; set; }
        [DisplayName("Care System Comment")]
        public string care_comment { get; set; }


    }

}