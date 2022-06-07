using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class LeaderModel
    {
        public string year { get; set; }
        public IEnumerable<SelectListItem> yearList { get; set; }
    }
}