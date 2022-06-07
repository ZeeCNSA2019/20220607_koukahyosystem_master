using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class HyoukaSaitem
    {
        public List<kubun> List_kubun { get; set; }
        [Display(Name = "日付")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        // public DateTime RequestDate { get; set; }
        public String RequestDate { get; set; }
        public String currentdate { get; set; }

        public DataTable dt_Hyouka { get; set; }
        public DataTable dt_Kijun { get; set; }
        public String limit_input { get; set; }//20210705
        public String btn_disabled { get; set; }//20210705
        public int input_maxlength { get; set; }//20210705
        public DataTable dt_HyoukaCode { get; set; }
        //  public List<hyoukasha> List_hyoukasha { get; set; }
        public List<string[]> List_hyoukasha { get; set; }
        public string cYear { get; set; }
        public string selectcode { get; set; }
        public string scode { get; set; }
        public string ccode { get; set; }
        public string sYear { get; set; }
        public string Year { set; get; }

        public IEnumerable<SelectListItem> yearList { get; set; }
    }
    public class kubun
    {
        public string cKUBUN { get; set; }
        public string sKUBUN { get; set; }


    }


    public class emp_table
    {

        public string Name { get; set; }

    }

}