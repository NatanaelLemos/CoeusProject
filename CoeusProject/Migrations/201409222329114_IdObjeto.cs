namespace CoeusProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdObjeto : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Grupo", name: "NidObjeto", newName: "IdObjeto");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Grupo", name: "IdObjeto", newName: "NidObjeto");
        }
    }
}
