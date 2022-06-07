using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace koukahyosystem.Models
{
    public class MasterBushoModel
    {
        public string kensaku { get; set; }

        public List<bushoMaster> BushoList { get; set; }

        public string kensuu { get; set; }
        public string bushomei { get; set; }

        public string deleCode { get; set; }

        public string code_lbl { get; set; }

        public string name_lbl { get; set; }

        public string gamenStr { get; set; }
        
    }
    public class bushoMaster {
        public string cbusho { get; set; }

        public string sbusho { get; set; }

        public string njunban { get; set; }

        public string active { get; set; }
    }
}