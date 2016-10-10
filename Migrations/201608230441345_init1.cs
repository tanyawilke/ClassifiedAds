namespace ClassifiedAdsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdsViewAdverts", "DateLastModified", c => c.DateTime());
            AlterColumn("dbo.AdsViewAdverts", "DateAdvertAdded", c => c.DateTime());
            AlterColumn("dbo.AdsViewAdverts", "DateOfDeleteAdvert", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AdsViewAdverts", "DateOfDeleteAdvert", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AdsViewAdverts", "DateAdvertAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AdsViewAdverts", "DateLastModified", c => c.DateTime(nullable: false));
        }
    }
}
