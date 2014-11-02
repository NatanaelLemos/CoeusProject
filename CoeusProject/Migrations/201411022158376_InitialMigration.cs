namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Artigo",
                c => new
                    {
                        IdArtigo = c.Int(nullable: false, identity: true),
                        TxArtigo = c.String(nullable: false),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdArtigo)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto, cascadeDelete: true)
                .Index(t => t.IdObjeto);
            
            CreateTable(
                "dbo.Objeto",
                c => new
                    {
                        IdObjeto = c.Int(nullable: false, identity: true),
                        NmObjeto = c.String(nullable: false, maxLength: 255),
                        TxDescricao = c.String(maxLength: 2048),
                        IdUsuario = c.Int(nullable: false),
                        IdSalt = c.Int(nullable: false),
                        QtAcessos = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdObjeto)
                .ForeignKey("dbo.Salt", t => t.IdSalt, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdSalt)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Avaliacao",
                c => new
                    {
                        IdAvaliacao = c.Int(nullable: false, identity: true),
                        NoAvaliacao = c.Int(nullable: false),
                        IdUsuario = c.Int(),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdAvaliacao)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario)
                .Index(t => t.IdObjeto)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        TxEmail = c.String(nullable: false, maxLength: 1024),
                        PwUsuario = c.String(nullable: false, maxLength: 1024),
                        NmPessoa = c.String(nullable: false, maxLength: 1024),
                        SnPessoa = c.String(nullable: false, maxLength: 1024),
                        NmFoto = c.String(maxLength: 100),
                        IdSalt = c.Int(nullable: false),
                        Usuario_IdUsuario = c.Int(),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .ForeignKey("dbo.Salt", t => t.IdSalt, cascadeDelete: false)
                .ForeignKey("dbo.Usuario", t => t.Usuario_IdUsuario)
                .Index(t => t.IdSalt)
                .Index(t => t.Usuario_IdUsuario);
            
            CreateTable(
                "dbo.Grupo",
                c => new
                    {
                        IdGrupo = c.Int(nullable: false, identity: true),
                        NmGrupo = c.String(nullable: false, maxLength: 512),
                        IdObjeto = c.Int(),
                        IdSalt = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdGrupo)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto)
                .ForeignKey("dbo.Salt", t => t.IdSalt, cascadeDelete: true)
                .Index(t => t.IdObjeto)
                .Index(t => t.IdSalt);
            
            CreateTable(
                "dbo.Mensagem",
                c => new
                    {
                        IdMensagem = c.Int(nullable: false, identity: true),
                        TxMensagem = c.String(nullable: false),
                        DtMensagem = c.DateTime(nullable: false),
                        IdUsuario = c.Int(nullable: false),
                        IdGrupo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdMensagem)
                .ForeignKey("dbo.Grupo", t => t.IdGrupo, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdGrupo)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Salt",
                c => new
                    {
                        IdSalt = c.Int(nullable: false, identity: true),
                        TxSalt = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdSalt);
            
            CreateTable(
                "dbo.Tema",
                c => new
                    {
                        IdTema = c.Int(nullable: false, identity: true),
                        NmTema = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdTema);
            
            CreateTable(
                "dbo.Imagem",
                c => new
                    {
                        IdImagem = c.Int(nullable: false, identity: true),
                        TxUrl = c.String(nullable: false),
                        IdArtigo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdImagem)
                .ForeignKey("dbo.Artigo", t => t.IdArtigo, cascadeDelete: true)
                .Index(t => t.IdArtigo);
            
            CreateTable(
                "dbo.Sequence",
                c => new
                    {
                        NmSequence = c.String(nullable: false, maxLength: 100),
                        VlSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NmSequence);
            
            CreateTable(
                "dbo.Video",
                c => new
                    {
                        IdVideo = c.Int(nullable: false, identity: true),
                        TxUrl = c.String(nullable: false),
                        TxUrlPoster = c.String(nullable: false),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdVideo)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto, cascadeDelete: true)
                .Index(t => t.IdObjeto);
            
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
            DropForeignKey("dbo.Video", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Imagem", "IdArtigo", "dbo.Artigo");
            DropForeignKey("dbo.Artigo", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Objeto", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Objeto", "IdSalt", "dbo.Salt");
            DropForeignKey("dbo.Avaliacao", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.TemaUsuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.TemaUsuario", "Tema_IdTema", "dbo.Tema");
            DropForeignKey("dbo.TemaObjeto", "Objeto_IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.TemaObjeto", "Tema_IdTema", "dbo.Tema");
            DropForeignKey("dbo.Usuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Usuario", "IdSalt", "dbo.Salt");
            DropForeignKey("dbo.GrupoUsuario", "Usuario_IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.GrupoUsuario", "Grupo_IdGrupo", "dbo.Grupo");
            DropForeignKey("dbo.Grupo", "IdSalt", "dbo.Salt");
            DropForeignKey("dbo.Grupo", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Mensagem", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Mensagem", "IdGrupo", "dbo.Grupo");
            DropForeignKey("dbo.Avaliacao", "IdObjeto", "dbo.Objeto");
            DropIndex("dbo.Video", new[] { "IdObjeto" });
            DropIndex("dbo.Imagem", new[] { "IdArtigo" });
            DropIndex("dbo.Artigo", new[] { "IdObjeto" });
            DropIndex("dbo.Objeto", new[] { "IdUsuario" });
            DropIndex("dbo.Objeto", new[] { "IdSalt" });
            DropIndex("dbo.Avaliacao", new[] { "IdUsuario" });
            DropIndex("dbo.TemaUsuario", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.TemaUsuario", new[] { "Tema_IdTema" });
            DropIndex("dbo.TemaObjeto", new[] { "Objeto_IdObjeto" });
            DropIndex("dbo.TemaObjeto", new[] { "Tema_IdTema" });
            DropIndex("dbo.Usuario", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.Usuario", new[] { "IdSalt" });
            DropIndex("dbo.GrupoUsuario", new[] { "Usuario_IdUsuario" });
            DropIndex("dbo.GrupoUsuario", new[] { "Grupo_IdGrupo" });
            DropIndex("dbo.Grupo", new[] { "IdSalt" });
            DropIndex("dbo.Grupo", new[] { "IdObjeto" });
            DropIndex("dbo.Mensagem", new[] { "IdUsuario" });
            DropIndex("dbo.Mensagem", new[] { "IdGrupo" });
            DropIndex("dbo.Avaliacao", new[] { "IdObjeto" });
            DropTable("dbo.TemaUsuario");
            DropTable("dbo.TemaObjeto");
            DropTable("dbo.GrupoUsuario");
            DropTable("dbo.Video");
            DropTable("dbo.Sequence");
            DropTable("dbo.Imagem");
            DropTable("dbo.Tema");
            DropTable("dbo.Salt");
            DropTable("dbo.Mensagem");
            DropTable("dbo.Grupo");
            DropTable("dbo.Usuario");
            DropTable("dbo.Avaliacao");
            DropTable("dbo.Objeto");
            DropTable("dbo.Artigo");
        }
    }
}
