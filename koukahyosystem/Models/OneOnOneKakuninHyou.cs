using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class OneOnOneKakuninHyou
    {
        public IEnumerable<SelectListItem> YearList { get; set; }
        public string cur_year { get; set; }
    }
}