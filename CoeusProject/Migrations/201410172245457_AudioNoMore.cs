namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AudioNoMore : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Som", "IdObjeto", "dbo.Objeto");
            DropIndex("dbo.Som", new[] { "IdObjeto" });
            DropTable("dbo.Som");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Som",
                c => new
                    {
                        IdSom = c.Int(nullable: false, identity: true),
                        TxUrl = c.String(nullable: false),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdSom);
            
            CreateIndex("dbo.Som", "IdObjeto");
            AddForeignKey("dbo.Som", "IdObjeto", "dbo.Objeto", "IdObjeto", cascadeDelete: true);
        }
    }
}
