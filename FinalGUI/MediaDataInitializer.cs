using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalGUI
{
    public class MediaDataInitializer : DropCreateDatabaseIfModelChanges<MediaData>
    {
        protected override void Seed(MediaData context)
        {
            var items = new List<MediaItem>
            {
                new MediaItem { Title = "Heartstopper", Author = "Alice Oseman", Category = "Fiction", Description = "LGTB+ graphic novel" },
                new MediaItem { Title = "The Name of the Wind", Author = "Paddy Rothfuss", Category = "Fiction", Description = "Epic fantasy novel" },
                new MediaItem { Title = "Interstellar", Author = "Christopher Nolan", Category = "Other Media", Description = "Epic sci-fi movie" },
                new MediaItem { Title = "A Hundred Years of Solitude", Author = "Gabriel García Márquez", Category = "Fiction", Description = "The maximum exponent of magic realism" },
                new MediaItem { Title = "Letter to D.", Author = "André Gorz", Category = "Non-fiction", Description = "An intimate good-bye letter" },
                new MediaItem { Title = "The Matrix", Author = "The Wachowskis", Category = "Other Media", Description = "Revolutionary action/sci-fi film" }
            };

            items.ForEach(i => context.MediaItems.Add(i));
            context.SaveChanges();

            context.Bookings.Add(new Booking
            {
                MediaItemId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            });

            context.SaveChanges();
        }
    }
}
