using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    public class BirthRecordFormModel
    {
        public zz_patient motherPatient { get; set; }
        public zz_patient fatherPatient { get; set; }
        public zz_patient childPatient { get; set; }
        public zz_birthRecord birthRecord { get; set; }
    }
}