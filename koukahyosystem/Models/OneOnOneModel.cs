using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace koukahyosystem.Models
{
    public class OneOnOneModel
    {
        #region  検索条件
        public IEnumerable<SelectListItem> YearList { get; set; }
        [Display(Name = "社員名")]
        public string Ken_taishosha { get; set; }
        [Display(Name = "目標")]
        public string Ken_sMOKUHYO { get; set; }

        [Display(Name = "送信済")]
        public bool Ken_fKANRYOU { get; set; }

        [Display(Name = "実施済を表示する")]
        public Boolean Ken_fKAKUTEI { get; set; }
        #endregion

        #region OneOnOne入力画面
        [Display(Name = "日付")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime dHIDUKE { get; set; }

        //[Display(Name = "実施時間")]
        //public string tKAISHI { get; set; }

        //public string tKANRYOU { get; set; }

        [Display(Name = "実施日")]
        public string dJISHIBI { get; set; }

        [Display(Name = "目標")]
        public string sMOKUHYO { get; set; }

        [Display(Name = "行なったタスク")]
        public string Actiontask { get; set; }

        [Display(Name = "困っていること")]
        public string Trouble_tantousha { get; set; }

        public string Trouble_Leader { get; set; }

        [Display(Name = "気づいたこと")]
        public string Awareness_tantousha { get; set; }

        public string Awareness_Leader { get; set; }

        [Display(Name = "フィードバック")]
        public string Feedback { get; set; }

        [Display(Name = "TODO")]
        public string Todo { get; set; }

        [Display(Name = "リーダーメモ")]
        public string Memo { get; set; }

        public bool fKANRYOU { get; set; }

        public Boolean fKAKUTEI { get; set; }

        public string msg { get; set; }

        public string prv_date { get; set; }

        public string prv_djishi { get; set; }

        public string prv_tema { get; set; }

        public string prv_taskaction { get; set; }

        public string prv_trouble { get; set; }
        public string prv_awareness { get; set; }

        public string prv_trouble_L { get; set; }
        public string prv_awareness_L { get; set; }

        public string prv_feedback { get; set; }


        public string prv_memo { get; set; }
        #endregion

        #region custome sort web grid
        public int pgindex { get; set; }
        public string sort { get; set; }

        public string sortdir { get; set; }
        //public string sortdir_num { get; set; }

        public string sortdir_date { get; set; }

        public string sortdir_staishosha { get; set; }
        public string sortdir_sMokuhyo { get; set; }

        public string sortdir_kanryou { get; set; }

        //public string sortdir_kakutei { get; set; }

        public string sortdir_djishibi { get; set; }
        #endregion
        public string cur_year { get; set; }
      
        public string status { get; set; }

        //public string Kubun { get; set; }
        public bool fSuperior { get; set; }

        public Boolean fpermit { get; set; }//permission input for current year

        public List<oneononList> OneOnOneList { get; set; }

        public string cTaishosha { get; set; }

        public string cMOKUHYO { get; set; }      

        public string GamenName { get; set; }
        public string btnName { get; set; }

        public bool fpopup { get; set; }

        public bool fprvpopup { get; set; }

        public string comfirmMsg { get; set; }

        public bool subpopup { get; set; }

        public bool fnew { get; set; }

    }
  
    public class oneononList
    {

        public string cTAISHOSHA { get; set; }
        public string sTAISHOSHA { get; set; }
        public string dHIDUKE { get; set; }
        public string cMOKUHYO { get; set; }
        public string sMOKUHYO { get; set; }
        public string fKANRYOU { get; set; }
        //public string fKAKUTEI { get; set; }
        public string dJISHIBI { get; set; }

    }

}