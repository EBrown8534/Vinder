namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpvoteSkipDownvoteToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Upvotes", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Skips", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Downvotes", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Downvotes");
            DropColumn("dbo.AspNetUsers", "Skips");
            DropColumn("dbo.AspNetUsers", "Upvotes");
        }
    }
}
