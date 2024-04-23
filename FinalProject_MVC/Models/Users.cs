using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace FinalProject_MVC.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] Image { get; set; }
        public int CategoryId { get; set; }
        public virtual Categories Category { get; set; }
    }

    public class UsersConfiguration : EntityTypeConfiguration<Users>
    {
        public UsersConfiguration()
        {
            HasRequired(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .WillCascadeOnDelete(false);

            Property(c => c.Image).IsOptional();
        }
    }
}