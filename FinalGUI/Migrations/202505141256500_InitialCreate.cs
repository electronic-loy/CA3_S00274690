namespace FinalGUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        MediaItemId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.MediaItems", t => t.MediaItemId, cascadeDelete: true)
                .Index(t => t.MediaItemId);
            
            CreateTable(
                "dbo.MediaItems",
                c => new
                    {
                        MediaItemId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Author = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.MediaItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "MediaItemId", "dbo.MediaItems");
            DropIndex("dbo.Bookings", new[] { "MediaItemId" });
            DropTable("dbo.MediaItems");
            DropTable("dbo.Bookings");
        }
    }
}
