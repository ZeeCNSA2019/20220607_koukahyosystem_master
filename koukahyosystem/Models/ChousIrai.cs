using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace koukahyosystem.Models
{
    public class ChousIrai
    {
        public String RequestDate { get; set; }
        public String jiki { get; set; }
        public String checkquest { get; set; }
        public String checkkijun { get; set; }

        public List<ChousIrai> ChousaList { get; set; }
        
        public string c_name { get; set; }
        public string c_kanji { get; set; }

        public string c_kaisu { get; set; }
    }
}