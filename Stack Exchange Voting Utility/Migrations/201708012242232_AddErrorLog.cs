namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddErrorLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContentLength = c.Int(nullable: false),
                        Encoding = c.String(),
                        ApplicationPath = c.String(),
                        HttpMethod = c.String(),
                        Headers = c.String(),
                        RawUrl = c.String(),
                        RequestedUrl = c.String(),
                        QueryString = c.String(),
                        Form = c.String(),
                        Cookies = c.String(),
                        Exception = c.String(),
                        StackTrace = c.String(),
                        ContentType = c.String(),
                        FilePath = c.String(),
                        IsAuthenticated = c.Boolean(nullable: false),
                        ServerVariables = c.String(),
                        RequestType = c.String(),
                        PathInfo = c.String(),
                        PhysicalPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ErrorLogs");
        }
    }
}
