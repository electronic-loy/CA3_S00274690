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
                txtDetails.Text = $"ID: {selected.MediaItemId}\n" +
                                  $"Title: {selected.Title}\n" +
                                  $"Author: {selected.Author}\n" +
                                  $"Category: {selected.Category}\n" +
                                  $"Description: {selected.Description}";
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

                MessageBox.Show("Booking successful!");
                LoadBookings(); // Refresh bookings list
            }
        }

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
    }
    }

