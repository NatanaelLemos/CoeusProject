namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sequence : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sequence",
                c => new
                    {
                        NmSequence = c.String(nullable: false, maxLength: 100),
                        VlSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NmSequence);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sequence");
        }
    }
}
