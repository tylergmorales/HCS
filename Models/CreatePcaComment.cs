using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ispProject.Models
{
    public class CreatePcaComment
    {
        public int commentId { get; set; }
        public int pca_id { get; set; }
        public Nullable<System.DateTime> datetimeadded { get; set; }
        public int comment_type_id { get; set; }
        public string comment { get; set; }
    }
}