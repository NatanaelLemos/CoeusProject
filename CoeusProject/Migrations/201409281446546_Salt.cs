namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Salt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Salt",
                c => new
                    {
                        IdSalt = c.Int(nullable: false, identity: true),
                        TxSalt = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdSalt);
            
            AddColumn("dbo.Objeto", "IdSalt", c => c.Int(nullable: false));
            AddColumn("dbo.Usuario", "IdSalt", c => c.Int(nullable: false));
            AddColumn("dbo.Grupo", "IdSalt", c => c.Int(nullable: false));
            CreateIndex("dbo.Grupo", "IdSalt");
            CreateIndex("dbo.Usuario", "IdSalt");
            CreateIndex("dbo.Objeto", "IdSalt");
            AddForeignKey("dbo.Grupo", "IdSalt", "dbo.Salt", "IdSalt", cascadeDelete: false);
            AddForeignKey("dbo.Usuario", "IdSalt", "dbo.Salt", "IdSalt", cascadeDelete: false);
            AddForeignKey("dbo.Objeto", "IdSalt", "dbo.Salt", "IdSalt", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Objeto", "IdSalt", "dbo.Salt");
            DropForeignKey("dbo.Usuario", "IdSalt", "dbo.Salt");
            DropForeignKey("dbo.Grupo", "IdSalt", "dbo.Salt");
            DropIndex("dbo.Objeto", new[] { "IdSalt" });
            DropIndex("dbo.Usuario", new[] { "IdSalt" });
            DropIndex("dbo.Grupo", new[] { "IdSalt" });
            DropColumn("dbo.Grupo", "IdSalt");
            DropColumn("dbo.Usuario", "IdSalt");
            DropColumn("dbo.Objeto", "IdSalt");
            DropTable("dbo.Salt");
        }
    }
}
