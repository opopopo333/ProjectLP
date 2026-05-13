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
using System.Data.Entity;
namespace ProjectLP.Pages
{
    /// <summary>
    /// Interaction logic for AddEditBookPage.xaml
    /// </summary>
    public partial class AddEditBookPage : Page
    {
        private Books _currentBook;

        public AddEditBookPage(Books selectedBook)
        {
            InitializeComponent();
            _currentBook = selectedBook ?? new Books();
            DataContext = _currentBook;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CbGenre.ItemsSource = Core.Context.Genres.ToList();

                if (_currentBook.Id != 0)
                {
                    TbTitle.Text = _currentBook.Title;
                    TbDescription.Text = _currentBook.Description;
                    TbContent.Text = _currentBook.Content;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbTitle.Text))
            {
                MessageBox.Show("Введите название!");
                return;
            }

            var selectedGenre = CbGenre.SelectedItem as Genres; 
            if (selectedGenre == null)
            {
                MessageBox.Show("Выберите жанр!");
                return;
            }

            try
            {
                _currentBook.Title = TbTitle.Text;
                _currentBook.Description = TbDescription.Text;
                _currentBook.Content = TbContent.Text;
                _currentBook.AuthorId = Core.CurrentUser.Id;

                if (_currentBook.Id == 0)
                {

                    _currentBook.IsFrozen = false;
                    Core.Context.Books.Add(_currentBook);
                }

                Core.Context.SaveChanges();
                MessageBox.Show("Успешно сохранено!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
