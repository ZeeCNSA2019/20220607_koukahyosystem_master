using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class MasterGroupModel
    {
        //[Display(Name = "部署")]
       public string depart_lbl { get; set; }
       public IEnumerable<SelectListItem> bushoList { get; set; }

        public string cBUSHO { get; set; }
        
        public string kensaku { get; set; }

        public List<groupMaster> GroupList { get; set; }

        public string kensuu { get; set; }
        public string groupmei { get; set; }

        public string deleCode { get; set; }

        public string code_lbl { get; set; }

        public string name_lbl { get; set; }

       public string gamenStr { get; set;  }

    }

    public class groupMaster
    {
        public string cgroup { get; set; }

        public string sgroup { get; set; }

        public string njunban { get; set; }

        public string active { get; set; }
    }
}