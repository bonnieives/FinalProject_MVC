using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class MessageModel
    {
        public int MessageId { get; set; }
        public int ApartmentId { get; set; }
        public int SenderId { get; set; }
        public string Description { get; set; }
    }
}