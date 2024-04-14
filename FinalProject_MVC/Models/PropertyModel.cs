using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class PropertyModel
    {
        public int PropertyId { get; set; }
        public int CivicNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public Province? Province { get; set; }
    }
}