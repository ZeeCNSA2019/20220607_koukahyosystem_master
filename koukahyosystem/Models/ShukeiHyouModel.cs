using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace koukahyosystem.Models
{
    public class ShukeiHyouModel
    {
        public List<shukeihyo> ShukeiList { get; set; }
        public List<kanrishukeihyo> KanriShukeiList { get; set; }
        public string cur_year { get; set; }
        public IEnumerable<SelectListItem> YearList { get; set; }

        public string status { get; set; }

        public string fjyoicol { get; set; }
    }
    public class shukeihyo {
        public string cSHAIN { get; set; }
        public string sSHAIN { get; set; }

        public string description { get; set; }
      
        public string hyouka360 { get; set; }
        public string kokahyou { get; set; }
        public string sandankaihyouka { get; set; }

        public string jyouikouka { get; set; }
        public string total { get; set; }
        public string cKUBUN { get; set; }
        public string txt_getdata { get; set; }

    }

    public class kanrishukeihyo
    {
        public string cSHAIN { get; set; }
        public string sSHAIN { get; set; }
        public string sBUSHO { get; set; }
        public string sGROUP { get; set; }
        public string sKUBUN { get; set; }

        public string description { get; set; }

        public string hyouka360 { get; set; }
        public string kokahyou { get; set; }
        public string sandankaihyouka { get; set; }

        public string jyouikouka { get; set; }
        public string total { get; set; }
        public string cKUBUN { get; set; }

        public string txt_jyoudata { get; set; }
        public int jyou_digit { get; set; }
        public int nhai_digit { get; set; }
    }
}