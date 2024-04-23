using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class UserApartmentsViewModel
    {
        public Users User {  get; set; }
        public List<Apartments> Apartments { get; set; }
    }
}