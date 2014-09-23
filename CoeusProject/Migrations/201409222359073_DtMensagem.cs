namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DtMensagem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mensagem", "DtMensagem", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mensagem", "DtMensagem");
        }
    }
}
