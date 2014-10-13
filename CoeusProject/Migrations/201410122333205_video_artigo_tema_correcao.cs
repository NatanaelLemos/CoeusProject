namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class video_artigo_tema_correcao : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tema", "Artigo_IdArtigo", "dbo.Artigo");
            DropForeignKey("dbo.Tema", "Video_IdVideo", "dbo.Video");
            DropIndex("dbo.Tema", new[] { "Artigo_IdArtigo" });
            DropIndex("dbo.Tema", new[] { "Video_IdVideo" });
            DropColumn("dbo.Tema", "Artigo_IdArtigo");
            DropColumn("dbo.Tema", "Video_IdVideo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tema", "Video_IdVideo", c => c.Int());
            AddColumn("dbo.Tema", "Artigo_IdArtigo", c => c.Int());
            CreateIndex("dbo.Tema", "Video_IdVideo");
            CreateIndex("dbo.Tema", "Artigo_IdArtigo");
            AddForeignKey("dbo.Tema", "Video_IdVideo", "dbo.Video", "IdVideo");
            AddForeignKey("dbo.Tema", "Artigo_IdArtigo", "dbo.Artigo", "IdArtigo");
        }
    }
}
