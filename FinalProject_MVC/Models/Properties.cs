using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public enum Province
    {
        AB, BC, MB, NB, NL, NS, NT, NU, ON, PE, QC, SK, YK
    }

    public class Properties
    {
        [Key]
        public int PropertyId { get; set; }
        public int CivicNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public Province? Province { get; set; }

    }
}
