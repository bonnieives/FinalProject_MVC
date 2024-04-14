using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class ContractModel
    {
        public int ContractId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public double Value { get; set; }

        [Range(1, 31, ErrorMessage = "Payday must be between 1 and 31.")]
        public int Payday { get; set; }
        public int TenantId { get; set; }
        public int ApartmentId { get; set; }

    }
}