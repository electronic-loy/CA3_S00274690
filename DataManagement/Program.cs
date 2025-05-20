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
                    db.SaveChanges();
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
