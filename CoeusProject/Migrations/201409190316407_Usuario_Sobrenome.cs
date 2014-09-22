namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Usuario_Sobrenome : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuario", "SnPessoa", c => c.String(nullable: false, maxLength: 1024));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuario", "SnPessoa");
        }
    }
}
