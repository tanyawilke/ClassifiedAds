namespace ClassifiedAdsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AdsViewAdverts", name: "UserName_Id", newName: "User");
            RenameIndex(table: "dbo.AdsViewAdverts", name: "IX_UserName_Id", newName: "IX_User");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AdsViewAdverts", name: "IX_User", newName: "IX_UserName_Id");
            RenameColumn(table: "dbo.AdsViewAdverts", name: "User", newName: "UserName_Id");
        }
    }
}
