namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Seguidores : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuario", "Usuario_IdUsuario", c => c.Int());
            CreateIndex("dbo.Usuario", "Usuario_IdUsuario");
            AddForeignKey("dbo.Usuario", "Usuario_IdUsuario", "dbo.Usuario", "IdUsuario");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Usuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropIndex("dbo.Usuario", new[] { "Usuario_IdUsuario" });
            DropColumn("dbo.Usuario", "Usuario_IdUsuario");
        }
    }
}
