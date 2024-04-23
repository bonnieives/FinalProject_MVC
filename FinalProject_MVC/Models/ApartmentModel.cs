using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class ApartmentModel
    {
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; }
        public double ApartmentValue { get; set; }
        public int ApartmentNumber { get; set; }
        public int OwnerId { get; set; }
        public int ManagerId { get; set; }
        public int PropertyId { get; set; }
        public int StatusId { get; set; }
        public byte[] Image { get; set; }
    }
}