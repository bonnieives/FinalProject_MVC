using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class Events
    {
        [Key]
        public int EventId { get; set; }
        public int ApartmentId { get; set; }
        public string Description { get; set; }

        [ForeignKey("Apartment")]
        public Apartments Apartment { get; set; }

        public class EventsConfiguration : EntityTypeConfiguration<Events>
        {
            public EventsConfiguration()
            {
                HasRequired(c => c.Apartment)
                    .WithMany()
                    .HasForeignKey(c => c.ApartmentId)
                    .WillCascadeOnDelete(false);
            }
        }
    }
}