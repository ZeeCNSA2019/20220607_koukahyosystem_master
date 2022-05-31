using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class MasterKisoModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]

        public List<Kiso> KisoList { get; set; }
        public List<KisoCopy> KisoCopyList { get; set; }
        public List<KisoMark> KisoMarkList { get; set; }
        public IEnumerable<SelectListItem> kubun_list { get; set; }
        public string kubun_name { get; set; }
        //public IEnumerable<SelectListItem> type_list { get; set; }
        [DataType(DataType.DateTime)]
        
        public IEnumerable<SelectListItem> yearList { get; set; }
        public string year { get; set; }

        public IEnumerable<SelectListItem> copy_yearList { get; set; }
        public string copy_year { get; set; }

        //public string type_name { get; set; }

        public IEnumerable<SelectListItem> junban_list { get; set; }
        public string junban_name { get; set; }
        public string question_name { get; set; }
        public string new_question { get; set; }
        public string question_count { get; set; }
        public string delete_question { get; set; }
        public string kubun_code { get; set; }
        public string kisohyouka_check { get; set; }
        public string kubunMark { get; set; }
        //public string kubunKijun { get; set; }
        public string kubunKijun { get; set; }
        public IEnumerable<SelectListItem> type_list { get; set; }

        //public string kijun_1 { get; set; }

        //public string kijun_2 { get; set; }
        public string click_search { get; set; }
        public string allow_year { get; set; }
        public string show_popup { get; set; }
        public string allow_btnCopy { get; set; }
        public string allow_btnNew { get; set; }
    }
    public class Kiso
    {
        public string k_qCode { get; set; }
        public string k_question { get; set; }
        public string k_junban { get; set; }
    }
    public class KisoCopy
    {
        public bool copy_chk { get; set; }
        public string c_qCode { get; set; }
        public string c_question { get; set; }
    }
    public class KisoMark
    {
        public string k_ckubun { get; set; }
        public string k_skubun { get; set; }
        public string k_mark { get; set; }
        public IEnumerable<SelectListItem> type_list { get; set; }

        public string k_type { get; set; }
    }
}