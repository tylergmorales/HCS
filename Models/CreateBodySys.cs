using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ispProject.Models
{
    public class CreateBodySys
    {
        public int care_sys_id { get; set; }
        public int pca_id { get; set; }
        public Nullable<System.DateTime> datetimeadded { get; set; }
        public int care_sys_type_id { get; set; }
        public Boolean wdl_ex { get; set; }
        public string care_sys_comment { get; set; }
    }
}