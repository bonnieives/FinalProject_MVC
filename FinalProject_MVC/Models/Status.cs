﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public enum StatusDescription
    {
        Available, Unavailable
    }
    public class Status
    {
        [Key]
        public int StatusId { get; set; }
        public StatusDescription? StatusDescription { get; set; }
    }
}