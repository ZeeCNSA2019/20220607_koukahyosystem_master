/*
* 作成者　: テテ
* 日付：20200914
* 機能　：考課表テーマ画面,考課表テーマ確定画面
* 作成したパラメータ：Session["LoginName"] ,TempData["com_msg"]
* 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class KoukaTemaModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public string year { get; set; }
        public IEnumerable<SelectListItem> yearList { get; set; }

        public List<tema_list> tema_tableList { get; set; }
        public string disable_tema { get; set; }
        public string leader_kakutei { get; set; }
        //public string kakutei_confirm { get; set; }
        public bool txt_require { get; set; }
        public string kubun_code { get; set; }

        public string shain_name { get; set; }
        public string tema_name1 { get; set; }
        public string tema_name2 { get; set; }
        public string tema_name3 { get; set; }
        public string tema_name4 { get; set; }
        public string tema_name5 { get; set; }

        public IEnumerable<SelectListItem> shain_list { get; set; }
        //public IEnumerable<SelectListItem> shainList { get; set; }

        public bool check { get; set; }
        public string check_allow { get; set; }
        public string hozone_disable { get; set; }
        public string kakutei_disable { get; set; }
        public bool haiten_disable_1 { get; set; }
        public bool haiten_disable_2 { get; set; }
        public bool haiten_disable_3 { get; set; }
        public bool haiten_disable_4 { get; set; }
        public bool haiten_disable_5 { get; set; }
        public bool tema_no1_enable { get; set; }
        public bool tema_no2_enable { get; set; }
        public bool tema_no3_enable { get; set; }
        public bool tema_no4_enable { get; set; }
        public bool tema_no5_enable { get; set; }
        public bool tensuu_hide { get; set; }
        public string master_haiten_mark { get; set; }
        public string haiten_mark { get; set; }
        public string tensuu_mark { get; set; }
        public bool lbl_kakutei { get; set; }
        public string sashimodoshi_disable { get; set; }
        public string tasuku1_exist { get; set; }
        public string tasuku2_exist { get; set; }
        public string tasuku3_exist { get; set; }
        public string tasuku4_exist { get; set; }
        public string tasuku5_exist { get; set; }
        public string show_sashimodoshi { get; set; }
        public string chk_saitenhouhou { get; set; }
    }
    public class tema_list
    {
        public string no_value { get; set; }
        public string tema_name_value { get; set; }
        public string tema_value { get; set; }
        public string haiten { get; set; }
        public string taseritsu { get; set; }
        public string tokuten { get; set; }
    }
}