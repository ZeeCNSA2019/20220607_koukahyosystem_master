using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class HomeModel
    {
        public string cur_year { get; set; }
        public IEnumerable<SelectListItem> YearList { get; set; }

        public List<shukeihyo> ShukeiList { get; set; }

        public bool fhyouka { get; set; }
        public string hyouka360_info { get; set; }
        public string tema_info { get; set; }

        public string jishi_info { get; set; }

        public string kiso_info { get; set; }

        //public string irai_count { get; set; }

        public string mazokudo_info { get; set; }

        public string oneonone_info { get; set; }
    }

    public class MainRegister
    {
        [Display(Name = "氏名")]
        [Required(ErrorMessage = "* 姓を入力してください。")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "* 名を入力してください。")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "* ユーザー名を入力してください。")]
        [Display(Name = "ユーザー名")]
        public string username { get; set; }

        [Required(ErrorMessage = "* メールを入力してください。")]
        [Display(Name = "メール")]
        public string mail { get; set; }

       
        [Display(Name = "役割")]
        public List<string> role { get; set; }

        [Required(ErrorMessage = "* 役割を入力してください。")]
        public string roleval { get; set; }

        [Required(ErrorMessage = "* 会社を入力してください。")]
        [Display(Name = "会社")]
        public string company { get; set; }

        
        [Display(Name = "国")]
        public List<string> Country { get; set; }
        [Required(ErrorMessage = "* 国を入力してください。")]
        public string Countryval { get; set; }


    }
}