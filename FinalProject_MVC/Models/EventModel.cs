using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class EventModel
    {
        public int EventId { get; set; }
        public int ApartmentId { get; set; }
        public string Description { get; set; }
    }
}