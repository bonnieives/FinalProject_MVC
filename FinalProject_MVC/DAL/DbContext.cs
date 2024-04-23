using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FinalProject_MVC.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using static FinalProject_MVC.Models.Events;

namespace FinalProject_MVC.DAL
{
    public class FinalProjectContext : DbContext
    {
        public FinalProjectContext() : base("FinalProjectContext")
        {
        }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Apartments> Apartments { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Properties> Properties { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ContractsConfiguration());
            modelBuilder.Configurations.Add(new ApartmentsConfiguration());
            modelBuilder.Configurations.Add(new AppointmentsConfiguration());
            modelBuilder.Configurations.Add(new MessagesConfiguration());
            modelBuilder.Configurations.Add(new EventsConfiguration());
            modelBuilder.Configurations.Add(new UsersConfiguration());

            base.OnModelCreating(modelBuilder);
        }

    }
}