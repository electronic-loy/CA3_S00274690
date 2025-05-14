using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace FinalGUI
{
    //First class is for items in the library in order to filter them
    public class MediaItem
    {
        public int MediaItemId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; } // Fiction, Non-fiction, Other Media
        public string Description { get; set; }
    }

    //This class will show and store all the info from a booking
    public class Booking
    {
        public int BookingId { get; set; }
        public int MediaItemId { get; set; }
        public MediaItem MediaItem { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class MediaData : DbContext
    {
        public MediaData() : base("MyMediaData") { }
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Booking> Bookings { get; set; }

    }
}
