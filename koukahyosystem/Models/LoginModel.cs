/*
 * 作成者　: ナン
 * 日付：20200424    
 * 機能　：ログイン画面
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace koukahyosystem.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "* ログイン名を入力してください。")]
        //[Display(Name = "ログイン名")]
        //public string loginName { get; set; }

        [Required(ErrorMessage = "* ユーザー名を入力してください。")]
        [Display(Name = "ユーザー名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "* パスワードを入力してください。")]
        [Display(Name = "パスワード")]
        public string psd { get; set; }

        [Required]
        [Display(Name = "ログイン名を保存")]
        public Boolean NameSave { get; set; }        
    }

}