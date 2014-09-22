namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FotoUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuario", "NmFoto", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuario", "NmFoto");
        }
    }
}
