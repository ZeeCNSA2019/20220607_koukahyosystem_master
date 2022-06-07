/*
 * 作成者　: ナン
 * 日付：20200424                                               
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class ShainModel
    {

        #region 検索条件項目
        [Display(Name = "社員No")]
        public string S_cSHAIN { get; set; }
        [Display(Name = "氏名")]
        public string S_sSHAIN { get; set; }

        public string S_busho_lbl { get; set; }
        public string S_cBUSHO { get; set; }
        [Display(Name = "考課区分")]
        public string S_cKUBUN { get; set; }

        [Display(Name = "退職")]
        public Boolean S_fTAISYA { get; set; }
        #endregion

        #region sorting 

        public string sort { get; set; }
        public string sortdir { get; set; }
        public string sortdir_cShain { get; set; }
        public string sortdir_sShain { get; set; }
        public string sortdir_slogin { get; set; }
        public string sortdir_kubun { get; set; }


        #endregion

        #region     社員マスタ入力画面

        [Required(ErrorMessage = "* 社員Noを入力してください。")]
        [Display(Name = "社員No")]
        public string cSHAIN { get; set; }

        [Required(ErrorMessage = "* ユーザー名を入力してください。")]
        [Display(Name = "ユーザー名")]
        public string sLOGIN { get; set; }

        [Required(ErrorMessage = "* 氏名を入力してください。")]
        [Display(Name = "氏名")]
        public string sSHAIN { get; set; }

        [Required(ErrorMessage = "* パスワードを入力してください。")]
        [Display(Name = "パスワード")]
        public string sPWD { get; set; }

        [Required(ErrorMessage = "* メールを入力してください。")]
        [Display(Name = "メール")]
        public string sMAIL { get; set; }

        [Display(Name = "役職")]
        public string sYAKUSHOKU { get; set; }

        [Required(ErrorMessage = "* 考課区分を入力してください。")]
        [Display(Name = "考課区分")]
        public string cKUBUN { get; set; }
        public string sKUBUN { get; set; }
        public IEnumerable<SelectListItem> kubunList { get; set; }

        //[Display(Name = "担当部署")]
        //public string stantoubusho { get; set; }
        //public List<Models.busho> tantoubusho { get; set; }

        [Display(Name = "評価者")]
        public string sHYOUKASHA { get; set; }

       public string count_taishousha { get; set; }

        [Required(ErrorMessage = "* 部署を入力してください。")]
        [Display(Name = "部署")]
        public string cBUSHO { get; set; }
        public IEnumerable<SelectListItem> bushoList { get; set; }

        public string Group_lbl { get; set; }
        public string cGROUP { get; set; }
        public IEnumerable<SelectListItem> groupList { get; set; }

        [Required(ErrorMessage = "* 前年度ランクを入力してください。")]
        [Display(Name = "前年度ランク")]
        public string cZENNENDORANK { get; set; }
        public IEnumerable<SelectListItem> zenendoList { get; set; }


        [Display(Name = "360度減点対象者")]
        public string nGENTEN { get; set; }
        public IEnumerable<SelectListItem> gentenList { get; set; }

        [Display(Name = "入社年月日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dNYUUSHA { get; set; }
        public string Pro_dNYUUSHA { get; set; }
        [Display(Name = "生年月日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> dSEINENGAPPI { get; set; }
        public string Pro_dSEINENGAPPI { get; set; }

        [Display(Name = "性別")]
        public string sSEIBETSU { get; set; }
        public IEnumerable<SelectListItem> sebetsuList { get; set; }

        [Display(Name = "退職")]
        public Boolean fTAISYA { get; set; }

        [Display(Name = "管理者")]
        public Boolean fKANRISYA { get; set; }
        //public Boolean fsave { get; set; }

        public HttpPostedFileBase ImgPath { get; set; }
        [Required(ErrorMessage = "* 画像をアップしてください。")]
        public string sPATH_GAZO { get; set; }
        #endregion

        public List<ShainModel> AllShainList { get; set; }

        public List<taishousha> taishoushaList { get; set; }

        public List<taishousha> Selecttaishosha { get; set; }

        public Boolean fnew { get; set; }

        public string gamenstatus { get; set; }

        public string cropImage { get; set; }
        public int pgindex { get; set; }

        public string taishoshaStr { get; set; }

        public string fckubun { get; set; }

        public bool fhyoukachu { get; set; }

        public bool fkiso { get; set; }

        public bool fmokuhyo { get; set; }

        public bool fhyouka { get; set; }

        public bool fimplemented { get; set; }

        #region

        [Display(Name = "検索")]
        public string mo_search { get; set; }

        [Display(Name = "部署")]
        public string mo_cBUSHO { get; set; }
        public IEnumerable<SelectListItem> mo_bushoList { get; set; }
        [Display(Name = "考課区分")]
        public string mo_cKUBUN { get; set; }
        public IEnumerable<SelectListItem> mo_kubunList { get; set; }
        [Display(Name = "グループ")]
        public string mo_group { get; set; }
        public IEnumerable<SelectListItem> mo_groupList { get; set; }
        #endregion

    }

    public class busho
    {
        public bool fchk { get; set; }
        public string cBUSHO { get; set; }
        public string sBUSHO { get; set; }
    }

    public class taishousha
    {
        public string ctaishousha { get; set; }
        public bool ftaishousha { get; set; }

        public string jyoutai { get; set; }
        public string staishousha { get; set; }

        public string hyukasha { get; set; }

        public string ckubun { get; set; }
        public string cgroup { get; set; }
        public string cbusho { get; set; }

    }
}