namespace FinalProject_MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApartmentTitle5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Apartments", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Apartments", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments");
            AddForeignKey("dbo.Apartments", "OwnerId", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.Apartments", "PropertyId", "dbo.Properties", "PropertyId", cascadeDelete: true);
            AddForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments", "ApartmentId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments");
            DropForeignKey("dbo.Apartments", "PropertyId", "dbo.Properties");
            DropForeignKey("dbo.Apartments", "OwnerId", "dbo.Users");
            AddForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments", "ApartmentId");
            AddForeignKey("dbo.Apartments", "PropertyId", "dbo.Properties", "PropertyId");
            AddForeignKey("dbo.Apartments", "OwnerId", "dbo.Users", "UserId");
        }
    }
}
