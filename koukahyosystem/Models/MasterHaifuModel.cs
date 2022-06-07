using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class MasterHaiFuModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "* 入力してください。")]
        public string year { get; set; }
        public IEnumerable<SelectListItem> yearList { get; set; }
        public string cKUBUN { get; set; }
        public string sKUBUN { get; set; }
        public IEnumerable<SelectListItem> kubunList { get; set; }
        public string cTYPE { get; set; }
        public string sTYPE { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public string sort_year{ get; set; }
        public string sort_kubun{ get; set; }
        //public string sort_type{ get; set; }
        //public string sort_mark{ get; set; }
        public string sort_type { get; set; }
        public string sort_mark { get; set; }
        public string sort_hyoukamark { get; set; }
        public string sort_kisomark { get; set; }
        public string sort_mokuhyomark { get; set; }
        public string sort_jyouimark { get; set; }
        public string sort_roundVal { get; set; }
        public string btn_txt{ get; set; }
        public bool fpopup { get; set; }
        public IEnumerable<SelectListItem> typeList { get; set; }

        public List<marks> HaiFuList { get; set; }
        public List<alltypes> AllTypeList { get; set; }

        public int pgindex { get; set; }

        public bool fpermit { get; set; }

       
    }
    public class alltypes
    {
        public string dNENDOU { get; set; }
        public string sKUBUN { get; set; }
        //public string sTYPE { get; set; }
        //public string nHAIFU { get; set; }
        public string hyoukamark { get; set; }
        public string kisomark { get; set; }
        public string mokuhyomark { get; set; }       
        public string jyouimark { get; set; }
        public string sROUNDING { get; set; }
      
    }
        public class marks
    {
        public string ckubun { get; set; }
        public string skubun { get; set; }
        public string kisomark { get; set; }
        public string temamark { get; set; }
        public string hyoukamark { get; set; }
        public string jyouimark { get; set; }
        public string roundVal { get; set; }
        //public string roundUp { get; set; }
        //public string roundDown { get; set; }
        //public string truncate { get; set; }
        public bool froundup { get; set; }
        public bool frounddown { get; set; }
        public bool ftruncate { get; set; }

        public string fhyoukacyuu { get; set; }
        public string fhenkou { get; set; }
    }
}