using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace koukahyosystem.Models
{
    public class SaitenModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        
        public string year { get; set; }
        public IEnumerable<SelectListItem> year_list { get; set; }

        public List<saitentable_lists> saiten_tableList { get; set; }

        public bool table_allow { get; set; }
    }

    public class saitentable_lists
    {
        public string no { get; set; }
        public string question { get; set; }
        public string jiki1 { get; set; }
        public string jiki2 { get; set; }
        public string jiki3 { get; set; }
        public string jiki4 { get; set; }
        public string total { get; set; }
        public string average { get; set; }

    }
}