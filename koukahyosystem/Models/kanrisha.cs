using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class kanrisha
    {
        public string cYear { get; set; }
        public string selectcode { get; set; }
        public string scode { get; set; }
        public string ccode { get; set; }
        public string sYear { get; set; }
        public string Year { set; get; }

        public IEnumerable<SelectListItem> yearList { get; set; }
    }
}