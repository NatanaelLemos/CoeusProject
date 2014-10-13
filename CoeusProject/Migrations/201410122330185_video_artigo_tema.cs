namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class video_artigo_tema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tema", "Artigo_IdArtigo", c => c.Int());
            AddColumn("dbo.Tema", "Video_IdVideo", c => c.Int());
            CreateIndex("dbo.Tema", "Artigo_IdArtigo");
            CreateIndex("dbo.Tema", "Video_IdVideo");
            AddForeignKey("dbo.Tema", "Artigo_IdArtigo", "dbo.Artigo", "IdArtigo");
            AddForeignKey("dbo.Tema", "Video_IdVideo", "dbo.Video", "IdVideo");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tema", "Video_IdVideo", "dbo.Video");
            DropForeignKey("dbo.Tema", "Artigo_IdArtigo", "dbo.Artigo");
            DropIndex("dbo.Tema", new[] { "Video_IdVideo" });
            DropIndex("dbo.Tema", new[] { "Artigo_IdArtigo" });
            DropColumn("dbo.Tema", "Video_IdVideo");
            DropColumn("dbo.Tema", "Artigo_IdArtigo");
        }
    }
}
