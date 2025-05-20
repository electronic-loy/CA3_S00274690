namespace FinalGUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImagePathToMediaItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaItems", "ImagePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaItems", "ImagePath");
        }
    }
}
