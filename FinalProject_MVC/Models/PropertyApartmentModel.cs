using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class PropertyApartmentModel
    {
        public PropertyModel Property { get; set; }
        public ApartmentModel Apartment { get; set; }
    }
}