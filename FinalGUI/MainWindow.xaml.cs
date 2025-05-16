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

namespace FinalGUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaData db = new MediaData();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Code for window load
            var categories = db.MediaItems
                                        .Select(m => m.Category)
                                        .Distinct()
                                        .ToList();

            categories.Insert(0, "All"); // Shows a selected filter or all items

            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedIndex = 0;

            // Load all media items into ListBox by default
            LoadMediaItems("All");
        }

        private void LoadMediaItems(string category)
        {
            var query = db.MediaItems.AsQueryable();

            if (category != "All")
            {
                query = query.Where(m => m.Category == category);
            }

            var items = query.ToList();
            
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = cmbCategory.SelectedItem as string;
            LoadMediaItems(selectedCategory);
        }

    }
    }

