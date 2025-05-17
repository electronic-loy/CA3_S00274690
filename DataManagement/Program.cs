using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FinalGUI;

namespace DataManagement 
{
    internal class Program 
    {

        static void Main(string[] args)
        {
            try
            {
                MediaData db = new MediaData();

                using (db)
                {

                    MediaItem i1 = new MediaItem
                    {
                        Title = "The Hobbit",
                        Author = "J.R.R. Tolkien",
                        Category = "Fiction",
                        Description = "A fantasy novel about Bilbo Baggins' adventure."
                    };

                    MediaItem i2 = new MediaItem
                    {
                        Title = "Sapiens",
                        Author = "Yuval Noah Harari",
                        Category = "Non-fiction",
                        Description = "A brief history of humankind."
                    };

                    MediaItem i3 = new MediaItem
                    {
                        Title = "Interstellar (DVD)",
                        Author = "Christopher Nolan",
                        Category = "Other Media",
                        Description = "A science fiction movie about space and time."
                    };

                    db.MediaItems.Add(i1);
                    db.MediaItems.Add(i2);
                    db.MediaItems.Add(i3);


                    Console.WriteLine("Added media items to database");

                    Booking booking1 = new Booking
                    {
                        MediaItemId = 1,
                        StartDate = new DateTime(2025, 5, 20),
                        EndDate = new DateTime(2025, 5, 25)
                    };

                    db.Bookings.Add(booking1);
                    Console.WriteLine("Added sample booking");

                    db.SaveChanges();

                    foreach (var item in db.MediaItems.ToList())
                    {
                        Console.WriteLine($"ID: {item.MediaItemId}, Title: {item.Title}");
                    }

                    Console.WriteLine("Done!");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("INNER: " + ex.InnerException.Message);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

    }
}
