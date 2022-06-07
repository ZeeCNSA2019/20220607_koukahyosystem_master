using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace koukahyosystem.Models
{
    public class MailInfo_Model
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public string address_val { get; set; }

        public string server_name { get; set; }

        public string port_no { get; set; }

        public string psw_val { get; set; }
    }
}