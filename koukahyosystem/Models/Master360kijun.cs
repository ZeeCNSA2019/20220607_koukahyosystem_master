using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class Master360kijun
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public IEnumerable<SelectListItem> m_kubunlist { get; set; }
        public string dd_kubuncode { get; set; }
        public string questname { get; set; }
        public string newquestname { get; set; }
        public List<m_questlist> m_Quest_List { get; set; }
        public List<kijun_copy_list> kijun_copy_list { get; set; }
        public IEnumerable<SelectListItem> jubanList { get; set; }
        public string selectjuban { get; set; }
        public string search_ddkubuncode { get; set; }
        public string save_allow { get; set; }
        public string searchname { get; set; }
        public string qname { get; set; }
        public string main_kname { get; set; }
        public string sort { get; set; }
        public string headername { get; set; }
        public string Year { set; get; }
        public string main_Year { set; get; }
        public string copy_Year { set; get; }
        public IEnumerable<SelectListItem> yearList { get; set; }
        public IEnumerable<SelectListItem> copy_yearList { get; set; }
    }
    public class m_questlist
    {
        public string alreadyuse { get; set; }
        public string kijun_code { get; set; }
        public string kijun_name { get; set; }
        public string njubun { get; set; }
        public string fdelete { get; set; }


    }
    public class kijun_copy_list
    {
        public bool fcopy { get; set; }
        public string q_copy_code { get; set; }
        public string q_copy_name { get; set; }
    }

}