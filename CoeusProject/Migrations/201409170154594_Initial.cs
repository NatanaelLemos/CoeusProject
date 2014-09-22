namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
                    })
                .PrimaryKey(t => t.IdObjeto)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        TxEmail = c.String(nullable: false, maxLength: 1024),
                        PwUsuario = c.String(nullable: false, maxLength: 1024),
                        NmPessoa = c.String(nullable: false, maxLength: 1024),
                    })
                .PrimaryKey(t => t.IdUsuario);
            
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
                "dbo.Grupo",
                c => new
                    {
                        IdGrupo = c.Int(nullable: false, identity: true),
                        NmGrupo = c.String(nullable: false, maxLength: 512),
                        NidObjeto = c.Int(),
                    })
                .PrimaryKey(t => t.IdGrupo)
                .ForeignKey("dbo.Objeto", t => t.NidObjeto)
                .Index(t => t.NidObjeto);
            
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
                "dbo.Mensagem",
                c => new
                    {
                        IdMensagem = c.Int(nullable: false, identity: true),
                        TxMensagem = c.String(nullable: false),
                        IdUsuario = c.Int(nullable: false),
                        IdGrupo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdMensagem)
                .ForeignKey("dbo.Grupo", t => t.IdGrupo, cascadeDelete: true)
                .ForeignKey("dbo.Usuario", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdGrupo)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Som",
                c => new
                    {
                        IdSom = c.Int(nullable: false, identity: true),
                        TxUrl = c.String(nullable: false),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdSom)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto, cascadeDelete: true)
                .Index(t => t.IdObjeto);
            
            CreateTable(
                "dbo.Tema",
                c => new
                    {
                        IdTema = c.Int(nullable: false, identity: true),
                        NmTema = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdTema);
            
            CreateTable(
                "dbo.Video",
                c => new
                    {
                        IdVideo = c.Int(nullable: false, identity: true),
                        TxUrl = c.String(nullable: false),
                        IdObjeto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdVideo)
                .ForeignKey("dbo.Objeto", t => t.IdObjeto, cascadeDelete: true)
                .Index(t => t.IdObjeto);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Video", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Som", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Mensagem", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Mensagem", "IdGrupo", "dbo.Grupo");
            DropForeignKey("dbo.Imagem", "IdArtigo", "dbo.Artigo");
            DropForeignKey("dbo.Grupo", "NidObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Avaliacao", "IdUsuario", "dbo.Usuario");
            DropForeignKey("dbo.Avaliacao", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Artigo", "IdObjeto", "dbo.Objeto");
            DropForeignKey("dbo.Objeto", "IdUsuario", "dbo.Usuario");
            DropIndex("dbo.Video", new[] { "IdObjeto" });
            DropIndex("dbo.Som", new[] { "IdObjeto" });
            DropIndex("dbo.Mensagem", new[] { "IdUsuario" });
            DropIndex("dbo.Mensagem", new[] { "IdGrupo" });
            DropIndex("dbo.Imagem", new[] { "IdArtigo" });
            DropIndex("dbo.Grupo", new[] { "NidObjeto" });
            DropIndex("dbo.Avaliacao", new[] { "IdUsuario" });
            DropIndex("dbo.Avaliacao", new[] { "IdObjeto" });
            DropIndex("dbo.Artigo", new[] { "IdObjeto" });
            DropIndex("dbo.Objeto", new[] { "IdUsuario" });
            DropTable("dbo.Video");
            DropTable("dbo.Tema");
            DropTable("dbo.Som");
            DropTable("dbo.Mensagem");
            DropTable("dbo.Imagem");
            DropTable("dbo.Grupo");
            DropTable("dbo.Avaliacao");
            DropTable("dbo.Usuario");
            DropTable("dbo.Objeto");
            DropTable("dbo.Artigo");
        }
    }
}
