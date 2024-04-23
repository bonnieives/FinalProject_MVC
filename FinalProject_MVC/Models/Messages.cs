using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FinalProject_MVC.Models
{
    public class Messages
    {
        [Key]
        public int MessageId { get; set; }
        public int ApartmentId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Description { get; set; }

        [ForeignKey("Apartment")]
        public Apartments Apartment { get; set; }

        [ForeignKey("Sender")]
        public Users Sender { get; set; }

        [ForeignKey("Receiver")]
        public Users Receiver { get; set; }

    }

    public class MessagesConfiguration : EntityTypeConfiguration<Messages>
    {
        public MessagesConfiguration()
        {
            HasRequired(c => c.Apartment)
                .WithMany()
                .HasForeignKey(c => c.ApartmentId)
                .WillCascadeOnDelete(true);

            HasRequired(c => c.Sender)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.Receiver)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .WillCascadeOnDelete(false);
        }
    }
}
