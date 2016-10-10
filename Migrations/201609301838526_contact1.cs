namespace ClassifiedAdsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contact1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdsViewContacts", "ContactId", "dbo.AdsViewAdverts");
            DropIndex("dbo.AdsViewContacts", new[] { "ContactId" });
            AddColumn("dbo.AdsViewContacts", "Id", c => c.Int(nullable: false));
            CreateIndex("dbo.AdsViewContacts", "Id");
            AddForeignKey("dbo.AdsViewContacts", "Id", "dbo.AdsViewAdverts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdsViewContacts", "Id", "dbo.AdsViewAdverts");
            DropIndex("dbo.AdsViewContacts", new[] { "Id" });
            DropColumn("dbo.AdsViewContacts", "Id");
            CreateIndex("dbo.AdsViewContacts", "ContactId");
            AddForeignKey("dbo.AdsViewContacts", "ContactId", "dbo.AdsViewAdverts", "Id");
        }
    }
}
