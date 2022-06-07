/*
 * 作成者　: ナン
 * 日付：20200424                                               
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace koukahyosystem.Models
{
    public class HyoukaIraiModel
    {
        public string jiki { get; set; }
      
        public List<HyoukaIrai> HyoukaIraiList { get; set; }

        [Required(ErrorMessage = "* 年度を入力してください。")]
        public string cur_year { get; set; }

        public bool f_premit { get; set; }

        public IEnumerable<SelectListItem> YearList { get; set; }

        [Required(ErrorMessage = "* 部署を入力してください。")]
       
        public string busho_lbl { get; set; }
        public string h_cBUSHO { get; set; }
        public IEnumerable<SelectListItem> h_bushoList { get; set; }

        public string group_lbl { get; set; }
        public string h_cGROUP { get; set; }
        public IEnumerable<SelectListItem> h_groupList { get; set; }

        public int Totalshain { get; set; }

        public string Dai1_BtnName { get; set; }
        public string Dai2_BtnName { get; set; }
        public string Dai3_BtnName { get; set; }
        public string Dai4_BtnName { get; set; }


        public string fDai1 { get; set; }
        public string fDai2 { get; set; }
        public string fDai3 { get; set; }
        public string fDai4 { get; set; }

      
    }

    public class HyoukaIrai
    {
        public string HyoukashaId { get; set; }
        public string hyoukasha { get; set; }   
        
        public string ftaisya { get; set; }
        public string dai_1 { get; set; }
        public bool f_chkDai1 { get; set; }
        public string dai_2 { get; set; }
        public bool f_chkDai2 { get; set; }
        public string dai_3 { get; set; }
        public bool f_chkDai3 { get; set; }
        public string dai_4 { get; set; }
        public bool f_chkDai4 { get; set; }

        public string fborder { get; set; }
    }
}