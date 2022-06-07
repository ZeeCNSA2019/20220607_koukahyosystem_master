using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class JyuyoutaskModel
    {
        [Display(Name = "年度：")]


        //[DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> testdate { get; set; }
        public string cBUSHO { get; set; }
        public string sBUSHO { get; set; }
        public IEnumerable<SelectListItem> tantoushabushoList { get; set; }

        [Display(Name = "氏名")]
        public string cShain { get; set; }
        public string sShain { get; set; }
        public IEnumerable<SelectListItem> cShainList { get; set; }

        public bool isActive { get; set; }
        public List<empmodel> temalist { get; set; }
        public List<kakunintasklist> kakunintemalist { get; set; }
        public List<allmonth> MonthList { get; set; }
        public List<allmonthvalue> MonthListValue { get; set; }

        [Required(ErrorMessage = "* 考課区分を入力してください。")]
        [Display(Name = "テーマ　")]
        public string cTEMA { get; set; }
        public string sTEMA { get; set; }
        public IEnumerable<SelectListItem> cTEMAList { get; set; }
        public string kubun { get; set; }

        public string selectyear { get; set; }
        public String minimonth { get; set; }
        public String maxmonth { get; set; }
        public String rdo_komoku { get; set; }
        public String visible { get; set; }

        public IEnumerable<SelectListItem> yearList { get; set; }
        //  public IEnumerable<SelectListItem> MonthList { get; set; }
        public string selectmonth { get; set; }
        public string upper_value { get; set; }
        public string lower_value { get; set; }
        public string kisyu_month { get; set; }
    }
    public class allmonth
    {
        public string selectmonth { get; set; }
    }
    public class allmonthvalue
    {
        public string selectmonthvalue { get; set; }
    }
    public class kakunintasklist
    {

        public String No { get; set; }

        public String Empid { get; set; }
        public String temaid { get; set; }

        public string Name { get; set; }

        public string year { get; set; }
        public String StartMonth { get; set; }
        public String EndMonth { get; set; }
        public string chkYear { get; set; }

        public string nHaitem { get; set; }
        public string result { get; set; }

        public string nTensu { get; set; }

        public string memo { get; set; }

        public string value1 { get; set; }

        public string value2 { get; set; }
        public string fkakutei { get; set; }
        public string kakuninsha { get; set; }
    }
    public class empmodel
    {

        public String No { get; set; }

        public String Empid { get; set; }
        public String temaid { get; set; }

        public string Name { get; set; }

        public string year { get; set; }
        public String StartMonth { get; set; }
        public String EndMonth { get; set; }
        public string chkYear { get; set; }

        public string nHaitem { get; set; }
        public string result { get; set; }

        public string nTensu { get; set; }

        public string memo { get; set; }

        public string value1 { get; set; }

        public string value2 { get; set; }
        public string fkakutei { get; set; }
        public string kakuninsha { get; set; }
    }
}