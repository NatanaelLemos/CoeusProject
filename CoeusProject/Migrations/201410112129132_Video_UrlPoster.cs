namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Video_UrlPoster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Video", "TxUrlPoster", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Video", "TxUrlPoster");
        }
    }
}
