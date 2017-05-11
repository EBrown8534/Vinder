namespace Stack_Exchange_Voting_Utility.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateTimeToDateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "AccessTokenExpiration", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "AccessTokenExpiration", c => c.DateTime());
        }
    }
}
