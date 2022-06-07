using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace koukahyosystem.Models
{
    public class MasterKubunModel
    {
        public List<kubun_list> KubunMasterList { get; set; }
        public bool isActive { get; set; }
        public string activerowcount { get; set; }
        public string kubunname { get; set; }
        public IEnumerable<SelectListItem> jubanList { get; set; }
        public string selectjuban { get; set; }
        public string selectkubunname { get; set; }
    }
    public class kubun_list
    {
        public string no_value { get; set; }
        public string alreadyuse { get; set; }
        public string kubun_code { get; set; }
        public string kubun_name { get; set; }
        public string njubun { get; set; }
        public string fdelete { get; set; }
        public bool f_chkDai1 { get; set; }

    }
}