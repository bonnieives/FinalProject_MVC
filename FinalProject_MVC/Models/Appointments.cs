using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class Appointments
    {
        [Key]
        public int AppointmentId { get; set; }
        public int ApartmentId { get; set; }
        public int SenderId { get; set; }
        public bool Confirmation { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("Appartment")]
        public Apartments Apartment { get; set; }

        [ForeignKey("User")]
        public Users User { get; set; }
    }

    public class AppointmentsConfiguration : EntityTypeConfiguration<Appointments>
    {
        public AppointmentsConfiguration()
        {
            HasRequired(c => c.Apartment)
                .WithMany()
                .HasForeignKey(c => c.ApartmentId)
                .WillCascadeOnDelete(true);

            HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .WillCascadeOnDelete(false);
        }
    }
}