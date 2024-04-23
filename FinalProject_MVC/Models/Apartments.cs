using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class Apartments
    {
        [Key]
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; }
        public double ApartmentValue { get; set; }
        public int ApartmentNumber { get; set; }
        public int OwnerId { get; set; }
        public int ManagerId { get; set; }
        public int PropertyId { get; set; }
        public int StatusId { get; set; }
        public byte[] Image { get; set; }

        [ForeignKey("Owner")]
        public virtual Users Owner { get; set; }

        [ForeignKey("Manager")]
        public virtual Users Manager { get; set; }

        [ForeignKey("Property")]
        public virtual Properties Property { get; set; }

        [ForeignKey("Status")]
        public virtual Status Status { get; set; }

    }
    public class ApartmentsConfiguration : EntityTypeConfiguration<Apartments>
    {
        public ApartmentsConfiguration()
        {
            HasRequired(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .WillCascadeOnDelete(true);

            HasRequired(c => c.Manager)
                .WithMany()
                .HasForeignKey(c => c.ManagerId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.Property)
                .WithMany()
                .HasForeignKey(c => c.PropertyId)
                .WillCascadeOnDelete(true);

            HasRequired(c => c.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusId)
                .WillCascadeOnDelete(false);

            Property(c => c.Image).IsOptional();
            Property(c => c.ApartmentTitle).IsOptional();
            Property(c => c.ApartmentValue).IsOptional();
        }
    }
}