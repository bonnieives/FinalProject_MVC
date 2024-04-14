namespace FinalProject_MVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _901 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "Contract_ContractId", "dbo.Contracts");
            DropForeignKey("dbo.Events", "Manager_UserId", "dbo.Users");
            DropIndex("dbo.Events", new[] { "Contract_ContractId" });
            DropIndex("dbo.Events", new[] { "Manager_UserId" });
            AddColumn("dbo.Events", "ApartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "ApartmentId");
            AddForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments", "ApartmentId");
            DropColumn("dbo.Events", "Contract_ContractId");
            DropColumn("dbo.Events", "Manager_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Manager_UserId", c => c.Int());
            AddColumn("dbo.Events", "Contract_ContractId", c => c.Int());
            DropForeignKey("dbo.Events", "ApartmentId", "dbo.Apartments");
            DropIndex("dbo.Events", new[] { "ApartmentId" });
            DropColumn("dbo.Events", "ApartmentId");
            CreateIndex("dbo.Events", "Manager_UserId");
            CreateIndex("dbo.Events", "Contract_ContractId");
            AddForeignKey("dbo.Events", "Manager_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Events", "Contract_ContractId", "dbo.Contracts", "ContractId");
        }
    }
}
