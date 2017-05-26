namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupForTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Theme", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Theme");
        }
    }
}
