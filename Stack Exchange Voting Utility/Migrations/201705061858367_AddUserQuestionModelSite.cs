namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserQuestionModelSite : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserQuestionModels");
            AddColumn("dbo.UserQuestionModels", "Site", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.UserQuestionModels", new[] { "UserId", "QuestionId", "Site" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserQuestionModels");
            DropColumn("dbo.UserQuestionModels", "Site");
            AddPrimaryKey("dbo.UserQuestionModels", new[] { "UserId", "QuestionId" });
        }
    }
}
