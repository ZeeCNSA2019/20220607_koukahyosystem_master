using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class MasterSaitenModel
    {
        #region 検索条件
        public string Ken_year { get; set; }
        public IEnumerable<SelectListItem> YearList { get; set; }

        [Display(Name = "区分")]
        public string Ken_cKBUN { get; set; }
        public IEnumerable<SelectListItem> kubunList { get; set; }
        #endregion
        #region sort
        public string sort { get; set; }
        public string sortdir { get; set; }
        public string sortyear { get; set; }
        public string sortkubun { get; set; }
        public string sorten { get; set; }
        #endregion

        public List<saitenhouhou>saitenlist { get; set; }

        public string btnName { get; set; }

        public bool fpopup { get; set; }

        public int pgindex { get; set; }

        public bool fpermit { get; set; }

        #region 入力画面
        
        //public string saitenVal { get; set; }
        //public string[] saitenStr { get; set; }

        public string nUPPERLIMIT { get; set;  }
        public string nLOWERLIMIT{ get; set; }
        public string settingval { get; set; }
        public string fhyoukacyuu { get; set; }

        public string fhenkou { get; set; }
        public bool fmokuhyou { get; set; }

        public bool fjuuyoutask { get; set; }
        public string ckubun { get; set; }
        public string skubun { get; set; }
        public List<MasterSaitenModel> saitenhouhouList { get; set; }
        public string errmsg { get; set; }

      

        #endregion
    }
    public class saitenhouhou
    {
        public string dNENDOU { get; set; }
        //public string cKUBUN { get; set; }
        public string sKUBUN { get; set; }
        public string Saiten { get; set; }

        public string nUPPERLIMIT { get; set; }

        public string nLOWERLIMIT { get; set; }

        

    }
}
