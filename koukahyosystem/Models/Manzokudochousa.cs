using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace koukahyosystem.Models
{
    public class Manzokudochousa
    {
        public DataTable dt_chousa { get; set; }
        public DataTable dt_Kijuns { get; set; }
        public string limit_input { get; set; }//20210705
        public string btn_disabled { get; set; }//20210705
        public int input_maxlength { get; set; }//20210705
        public DataTable dt_suggest { get; set; }

        public String RequestDate { get; set; }
        public String jiki { get; set; }
        public string cYear { get; set; }
        public string selectcode { get; set; }
        public string scode { get; set; }
        public string ccode { get; set; }
        public string sYear { get; set; }
        public string Year { set; get; }
        public String currentdate { get; set; }
        public IEnumerable<SelectListItem> yearList { get; set; }
        public SelectList Actions { get; set; }


    }


}