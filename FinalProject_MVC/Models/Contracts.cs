using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FinalProject_MVC.Models
{
    public class Contracts
    {
        [Key]
        public int ContractId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }

        [Range(1, 31, ErrorMessage = "Payday must be between 1 and 31.")]
        public int Payday { get; set; }

        [ForeignKey("Tenant")]
        public int TenantId { get; set; }
        public virtual Users Tenant { get; set; }

        [ForeignKey("Apartment")]
        public int ApartmentId { get; set; }
        public virtual Apartments Apartment { get; set; }

    }

    public class ContractsConfiguration : EntityTypeConfiguration<Contracts>
    {
        public ContractsConfiguration()
        {
            HasRequired(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.Apartment)
                .WithMany()
                .HasForeignKey(c => c.ApartmentId)
                .WillCascadeOnDelete(true);

        }
    }
}
