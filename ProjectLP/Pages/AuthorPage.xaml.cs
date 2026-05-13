using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ProjectLP.Pages
{
    /// <summary>
    /// Interaction logic for AuthorPage.xaml
    /// </summary>
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
        }
        private void RefreshData()
        {
            DgAuthorBooks.ItemsSource = Core.Context.Books
                .Where(b => b.AuthorId == Core.CurrentUser.Id)
                .ToList();
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditBookPage(null));
        }

        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = (sender as Button).DataContext as Books;
            if (selectedBook != null)
            {
                NavigationService.Navigate(new AddEditBookPage(selectedBook));
            }
        }



        private void BtnEditText_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = (sender as Button).DataContext as Books;

            if (selectedBook != null)
            {
                MessageBox.Show($"Здесь будет открыто окно редактирования текста для книги: {selectedBook.Title}");
            }
        }
    }
}
