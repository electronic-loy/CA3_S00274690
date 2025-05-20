using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace FinalGUI
{
    
    public partial class MainWindow : Window
    {

        MediaData db = new MediaData();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var categories = db.MediaItems
                                   .Select(m => m.Category)
                                   .Distinct()
                                   .ToList();

                categories.Insert(0, "All");

                CmbCategory.ItemsSource = categories;
                CmbCategory.SelectedIndex = 0;

                LoadMediaItems("All");
                LoadBookings();
                LoadItemSummaries();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup Error: " + ex.Message);
            }
        }

        private void LoadMediaItems(string category)
        {
            var query = db.MediaItems.AsQueryable();

            if (category != "All")
            {
                query = query.Where(m => m.Category == category);
            }

            var items = query.ToList();
            LstItems.ItemsSource = items;
        }

        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = CmbCategory.SelectedItem as string;
            LoadMediaItems(selectedCategory);
        }
        //Search button in tab 1
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MediaData())
            {
                string selectedCategory = CmbCategory.SelectedItem?.ToString();
                DateTime? startDate = dpStart.SelectedDate;
                DateTime? endDate = dpEnd.SelectedDate;

                if (startDate == null || endDate == null || endDate < startDate)
                {
                    MessageBox.Show("Please enter valid start and end dates.");
                    return;
                }

                var items = db.MediaItems.Include(m => m.Category).ToList();

                if (selectedCategory != "All")
                {
                    items = items.Where(m => m.Category == selectedCategory).ToList();
                }

                var unavailableIds = db.Bookings
                    .Where(b =>
                        (startDate <= b.EndDate && endDate >= b.StartDate)
                    )
                    .Select(b => b.MediaItemId)
                    .ToList();

                var availableItems = items
                    .Where(m => !unavailableIds.Contains(m.MediaItemId))
                    .ToList();

                LstItems.ItemsSource = availableItems;
            }
        }
        //List details UX in tab 1
        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstItems.SelectedItem is MediaItem selected)
            {
                // Reload the most up-to-date item from the database
                using (var db = new MediaData())
                {
                    var freshItem = db.MediaItems.FirstOrDefault(m => m.MediaItemId == selected.MediaItemId);
                    if (freshItem != null)
                    {
                        txtDetails.Text = $"ID: {freshItem.MediaItemId}\n" +
                                          $"Title: {freshItem.Title}\n" +
                                          $"Author: {freshItem.Author}\n" +
                                          $"Category: {freshItem.Category}\n" +
                                          $"Description: {freshItem.Description}";

                        if (!string.IsNullOrEmpty(freshItem.ImagePath) && System.IO.File.Exists(freshItem.ImagePath))
                        {
                            ImgItem.Source = new BitmapImage(new Uri(freshItem.ImagePath));
                        }
                        else
                        {
                            ImgItem.Source = null;
                        }
                    }
                }
            }
        }

        //Upload an image
        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            if (LstItems.SelectedItem is MediaItem selectedMediaItem)
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "Image files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";


                if (dialog.ShowDialog() == true)
                {
                    string imagePath = dialog.FileName;

                    using (var db = new MediaData())
                    {
                        var item = db.MediaItems.Find(selectedMediaItem.MediaItemId);
                        if (item != null)
                        {
                            item.ImagePath = imagePath;
                            db.SaveChanges();
                            MessageBox.Show("Image added successfully.");
                        }

                    }
                    string currentCategory = CmbCategory.SelectedItem as string ?? "All";
                    LoadMediaItems(currentCategory);
                    LstItems.SelectedItem = LstItems.Items
                                                        .OfType<MediaItem>()
                                                            .FirstOrDefault(m => m.MediaItemId == selectedMediaItem.MediaItemId);
                    ImgItem.Source = new BitmapImage(new Uri(imagePath));
                }
            }

            else
            {
                MessageBox.Show("Please select a media item first.");
            }
            

        }

        //Booking action in tab 1
        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            if (!(LstItems.SelectedItem is MediaItem selected))
            {
                MessageBox.Show("Select an item first.");
                return;
            }


            DateTime? startDate = dpStart.SelectedDate;
            DateTime? endDate = dpEnd.SelectedDate;

            if (startDate == null || endDate == null || endDate < startDate)
            {
                MessageBox.Show("Please enter valid dates.");
                return;
            }

            if (startDate < DateTime.Today || endDate < DateTime.Today)
            {
                MessageBox.Show("Dates must be from today onwards.");
                return;
            }

            // Unable to book an item in a "rented" day
            var conflicts = db.Bookings.Where
                                        (b => b.MediaItemId == selected.MediaItemId &&
                                        startDate <= b.EndDate && endDate >= b.StartDate)
                                        .ToList();

            if (conflicts.Any())
            {
                MessageBox.Show("This item is already booked for the selected date range.");
                return;
            }
            using (var db = new MediaData())
            {
                var newBooking = new Booking
                {
                    MediaItemId = selected.MediaItemId,
                    StartDate = startDate.Value,
                    EndDate = endDate.Value
                };

                db.Bookings.Add(newBooking);
                db.SaveChanges();

                //Confirmation MessageBox
                MessageBox.Show($"Booking confirmation:\n" +
                                $"Media ID: {selected.MediaItemId}\n" +
                                $"Name: {selected.Title}\n" +
                                $"Author: {selected.Author}\n" +
                                $"Rental Date: {startDate:dd/MM/yyyy}\n" +
                                $"Return Date: {endDate:dd/MM/yyyy}");
                LoadBookings(); // Refresh bookings list
                //Refreshes Tab 2
                LoadItemSummaries();
            }
        }
        // Tab 2 show the list
        private void LoadItemSummaries()
        {
            using (var db = new MediaData())
            {
                var summaries = db.MediaItems
                    .Select(m => new MediaItemSummary
                    {
                        ID = m.MediaItemId,
                        Name = m.Title,
                        Author = m.Author,
                        BookingCount = m.Bookings.Count()
                    })
                    .ToList();

                dgItems.ItemsSource = summaries;
            }
        }




        //Delete a booking in Tab 3
        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgBookings.SelectedItem is Booking selected))
            {
                MessageBox.Show("Select a booking to delete.");
                return;
            }

            using (var db = new MediaData())
            {
                var bookingToDelete = db.Bookings.Find(selected.BookingId);
                if (bookingToDelete != null)
                {
                    db.Bookings.Remove(bookingToDelete);
                    db.SaveChanges();
                    MessageBox.Show("Booking deleted.");
                    LoadBookings();
                }
            }
        }
        private void LoadBookings()
        {
            using (var db = new MediaData())
            {
                dgBookings.ItemsSource = db.Bookings
                    .Include(b => b.MediaItem)
                    .ToList();
            }
        }

        //Reference for Tab 2
        public class MediaItemSummary
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Author { get; set; }
            public int BookingCount { get; set; }
        }
    }
    }

