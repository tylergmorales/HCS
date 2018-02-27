using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    public class PCAHistoryDetail
    {
        public int pcaId { get; set; }
        public int pcaHistoryId { get; set; }
        public int encounterId { get; set; }
        //public string patient_name { get; set; }
        //public virtual patient patient { get; set; }
        //public string birthdate { get; set; }
        //public string mrn { get; set; }
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
        public string pcaComment1 { get; set; }
        public string pcaComment2 { get; set; }
        public string pcaComment3 { get; set; }
        public string pcaComment4 { get; set; }
        public string pcaComment5 { get; set; }
        public string wdl { get; set; }
        public string wdl1 { get; set; }
        public string wdl2 { get; set; }
        public string wdl3 { get; set; }
        public string wdl4 { get; set; }
        public string wdl5 { get; set; }
        public string wdl6 { get; set; }
        public string wdl7 { get; set; }
        public string wdl8 { get; set; }
        public string wdl9 { get; set; }
        public string wdl10 { get; set; }
        public string wdl11 { get; set; }
        public string wdl12 { get; set; }
        public string wdl13 { get; set; }
        public Nullable<int> pain_scale_type_id { get; set; }
        public string pain_scale_type { get; set; }
        public byte pain_level_actual { get; set; }
        public byte pain_level__goal { get; set; }
        public string painComment { get; set; }
        public string bodysystemsComment { get; set; }
        public string bodysystemsComment1 { get; set; }
        public string bodysystemsComment2 { get; set; }
        public string bodysystemsComment3 { get; set; }
        public string bodysystemsComment4 { get; set; }
        public string bodysystemsComment5 { get; set; }
        public string bodysystemsComment6 { get; set; }
        public string bodysystemsComment7 { get; set; }
        public string bodysystemsComment8 { get; set; }
        public string bodysystemsComment9 { get; set; }
        public string bodysystemsComment10 { get; set; }
        public string bodysystemsComment11 { get; set; }
        public string bodysystemsComment12 { get; set; }
        public string bodysystemsComment13 { get; set; }
        public DateTime bodysystemsDateAdded { get; set; }
        public DateTime bodysystemsDateAdded2 { get; set; }
        public DateTime bodysystemsDateAdded3 { get; set; }
        public DateTime bodysystemsDateAdded4 { get; set; }
        public DateTime bodysystemsDateAdded5 { get; set; }
        public DateTime bodysystemsDateAdded6 { get; set; }
        public DateTime bodysystemsDateAdded7 { get; set; }
        public DateTime bodysystemsDateAdded8 { get; set; }
        public DateTime bodysystemsDateAdded9 { get; set; }
        public DateTime bodysystemsDateAdded10 { get; set; }
        public DateTime bodysystemsDateAdded11 { get; set; }
        public DateTime bodysystemsDateAdded12 { get; set; }
        public DateTime bodysystemsDateAdded13 { get; set; }
        public DateTime pcaCommentDateAdded { get; set; }
        public DateTime pcacommentDateAdded2 { get; set; }
        public DateTime pcacommentDateAdded3 { get; set; }
        public DateTime pcacommentDateAdded4 { get; set; }
        public DateTime pcacommentDateAdded5 { get; set; }
        public string des1 { get; set; }
        public string des2 { get; set; }
        public string des3 { get; set; }
        public string des4 { get; set; }
        public string des5 { get; set; }
        public string des6 { get; set; }
        public string des7 { get; set; }
        public string des8 { get; set; }
        public string des9 { get; set; }
        public string des10 { get; set; }
        public string des11 { get; set; }
        public string des12 { get; set; }
        public string des13 { get; set; }

        public DateTime dateModified { get; set; }
        public DateTime dateOriginal { get; set; }


    }
}