using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class AppointmentModel
    {
        public int AppointmentId { get; set; }
        public int ApartmentId { get; set; }
        public int SenderId { get; set; }
        public bool Confirmation { get; set; }
        public DateTime DateTime { get; set; }

    }
}