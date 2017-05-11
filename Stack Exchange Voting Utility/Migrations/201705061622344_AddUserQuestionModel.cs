namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserQuestionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserQuestionModels",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        QuestionId = c.Int(nullable: false),
                        Action = c.String(),
                    })
                .PrimaryKey(t => new { t.UserId, t.QuestionId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserQuestionModels", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserQuestionModels", new[] { "UserId" });
            DropTable("dbo.UserQuestionModels");
        }
    }
}
