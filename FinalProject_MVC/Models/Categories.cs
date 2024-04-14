using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public enum Category
    {
        Administrator = 4, Owner = 5, Tenant = 6, Manager = 7
    }
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        public Category? Description { get; set; }
    }
}