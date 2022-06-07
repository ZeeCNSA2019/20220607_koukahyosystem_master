using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class MasterKokatenModel
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

        public List<kokaten> kokatenlist { get; set; }

        public string btnName { get; set; }

        public bool fpopup { get; set; }

        public int pgindex { get; set; }

        public bool fpermit { get; set; }

        #region 入力画面
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        //[Required(ErrorMessage = "* 入力してください。")]
        public string mark { get; set; }
        public string ckubun { get; set; }
        public string skubun { get; set; }
        public List<MasterKokatenModel> MarkList { get; set; }
        public string errmsg { get; set; }
        #endregion
        
    }
    public class kokaten
    {
        public string dNENDO { get; set; }
        //public string cKUBUN { get; set; }
        public string sKUBUN { get; set; }
        public string nMARK { get; set; }

    }
}
