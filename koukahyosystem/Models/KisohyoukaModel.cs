using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class KisohyoukaModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        
        public List<monthTable_lists> shinsei_tableList_month { get; set; }

        public List<yearTable_lists> shinsei_tableList_year { get; set; }
       // public List<order_lists> monthOrder_list { get; set; }
        //public List<order_lists> monthOrder_list { get; set; }
        public List<string> monthList { get; set; }

        [DataType(DataType.DateTime)]
        public string year { get; set; }
        
        public IEnumerable<SelectListItem> yearList { get; set; }

        public string disable_mth4 { get; set; }
        public string disable_mth5 { get; set; }
        public string disable_mth6 { get; set; }
        public string disable_mth7 { get; set; }
        public string disable_mth8 { get; set; }
        public string disable_mth9 { get; set; }
        public string disable_mth10 { get; set; }
        public string disable_mth11 { get; set; }
        public string disable_mth12 { get; set; }
        public string disable_mth1 { get; set; }
        public string disable_mth2 { get; set; }
        public string disable_mth3 { get; set; }
        public string disable_txtyear { get; set; }
        public string savebtn_disable { get; set; }

        public string leaderKakutei_mth4 { get; set; }
        public string leaderKakutei_mth5 { get; set; }
        public string leaderKakutei_mth6 { get; set; }
        public string leaderKakutei_mth7 { get; set; }
        public string leaderKakutei_mth8 { get; set; }
        public string leaderKakutei_mth9 { get; set; }
        public string leaderKakutei_mth10 { get; set; }
        public string leaderKakutei_mth11 { get; set; }
        public string leaderKakutei_mth12 { get; set; }
        public string leaderKakutei_mth1 { get; set; }
        public string leaderKakutei_mth2 { get; set; }
        public string leaderKakutei_mth3 { get; set; }
        public string leaderKakutei_txtyear { get; set; }

        public List<tabs> tabList { get; set; }

        //public bool showTab { get; set; }
        public string showTab { get; set; }
        public bool disableBtn { get; set; }
        public string show_table { get; set; }
        public string txt_kijun { get; set; }
        public string txt_mark { get; set; }
        public string markLabel { get; set; }
    }
    
    public class tabs
    {
        public string tabName { get; set; }
        public string tabId { get; set; }
    }
   
    public class monthTable_lists
    {
        public string no_value { get; set; }
        public string question { get; set; }
        public string question_code { get; set; }
        public string four { get; set; }
        public string five { get; set; }
        public string six { get; set; }
        public string seven { get; set; }
        public string eight { get; set; }
        public string nine { get; set; }
        public string ten { get; set; }
        public string eleven { get; set; }
        public string twelve { get; set; }
        public string one { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string total { get; set; }
        
    }

    public class yearTable_lists
    {
        public string no_value { get; set; }
        public string question { get; set; }
        public string question_code { get; set; }
        public string year_value { get; set; }
    }
    //public class order_lists
    //{
    //    public string monthVal { get; set; }
    //}
}