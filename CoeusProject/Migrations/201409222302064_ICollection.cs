namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ICollection : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrupoUsuario",
                c => new
                    {
                        Grupo_IdGrupo = c.Int(nullable: false),
                        Usuario_IdUsuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Grupo_IdGrupo, t.Usuario_IdUsuario })
                .ForeignKey("dbo.Grupo", t => t.Grupo_IdGrupo)
                .ForeignKey("dbo.Usuario", t => t.Usuario_IdUsuario)
                .Index(t => t.Grupo_IdGrupo)
                .Index(t => t.Usuario_IdUsuario);
            
            CreateTable(
                "dbo.TemaObjeto",
                c => new
                    {
                        Tema_IdTema = c.Int(nullable: false),
                        Objeto_IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tema_IdTema, t.Objeto_IdObjeto })
                .ForeignKey("dbo.Tema", t => t.Tema_IdTema)
                .ForeignKey("dbo.Objeto", t => t.Objeto_IdObjeto)
                .Index(t => t.Tema_IdTema)
                .Index(t => t.Objeto_IdObjeto);
            
            CreateTable(
                "dbo.TemaUsuario",
                c => new
                    {
                        Tema_IdTema = c.Int(nullable: false),
                        Usuario_IdUsuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tema_IdTema, t.Usuario_IdUsuario })
                .ForeignKey("dbo.Tema", t => t.Tema_IdTema)
                .ForeignKey("dbo.Usuario", t => t.Usuario_IdUsuario)
                .Index(t => t.Tema_IdTema)
                .Index(t => t.Usuario_IdUsuario);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemaUsuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.TemaUsuario", "Tema_IdTema", "dbo.Tema");
            DropForeignKey("dbo.TemaObjeto", "Objeto_IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.TemaObjeto", "Tema_IdTema", "dbo.Tema");
            DropForeignKey("dbo.GrupoUsuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.GrupoUsuario", "Grupo_IdGrupo", "dbo.Grupo");
            DropIndex("dbo.TemaUsuario", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.TemaUsuario", new[] { "Tema_IdTema" });
            DropIndex("dbo.TemaObjeto", new[] { "Objeto_IdObjeto" });
            DropIndex("dbo.TemaObjeto", new[] { "Tema_IdTema" });
            DropIndex("dbo.GrupoUsuario", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.GrupoUsuario", new[] { "Grupo_IdGrupo" });
            DropTable("dbo.TemaUsuario");
            DropTable("dbo.TemaObjeto");
            DropTable("dbo.GrupoUsuario");
        }
    }
}
