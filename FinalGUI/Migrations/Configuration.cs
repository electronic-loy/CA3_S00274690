namespace FinalGUI.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinalGUI.MediaData>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FinalGUI.MediaData";
        }

        protected override void Seed(FinalGUI.MediaData context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            var mediaItems = new List<MediaItem>
    {
        new MediaItem { Title = "Heartstopper", Author = "Alice Oseman", Category = "Fiction", Description = "LGTB+ graphic novel" },
        new MediaItem { Title = "The Name of the Wind", Author = "Paddy Rothfuss", Category = "Fiction", Description = "Epic fantasy novel" },
        new MediaItem { Title = "Interstellar", Author = "Christopher Nolan", Category = "Other Media", Description = "Epic sci-fi movie" },
        new MediaItem { Title = "A Hundred Years of Solitude", Author = "Gabriel García Márquez", Category = "Fiction", Description = "The maximum exponent of magic realism" },
        new MediaItem { Title = "Letter to D.", Author = "André Gorz", Category = "Non-fiction", Description = "An intimate good-bye letter" },
        new MediaItem { Title = "The Matrix", Author = "The Wachowskis", Category = "Other Media", Description = "Revolutionary action/sci-fi film" },
        new MediaItem { Title = "The Hobbit", Author = "J.R.R. Tolkien", Category = "Fiction", Description = "A fantasy novel about Bilbo Baggins' adventure." },
        new MediaItem { Title = "Sapiens", Author = "Yuval Noah Harari", Category = "Non-fiction", Description = "A brief history of humankind." },
        new MediaItem { Title = "Interstellar (DVD)", Author = "Christopher Nolan", Category = "Other Media", Description = "A science fiction movie about space and time." }
    };

            foreach (var item in mediaItems)
            {
                context.MediaItems.AddOrUpdate(p => p.Title, item);
            }
            context.SaveChanges();

            // Add initial Bookings, referencing existing MediaItems by Title
            context.Bookings.AddOrUpdate(
                b => b.BookingId,
                new Booking
                {
                    MediaItemId = context.MediaItems.First(m => m.Title == "The Hobbit").MediaItemId,
                    StartDate = new DateTime(2025, 5, 20),
                    EndDate = new DateTime(2025, 5, 25)
                },
                new Booking
                {
                    MediaItemId = context.MediaItems.First(m => m.Title == "Sapiens").MediaItemId,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(2)
                }
            // Add more initial bookings as needed
            );
            context.SaveChanges();
        }
    }
}
