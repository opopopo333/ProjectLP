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
    /// Interaction logic for CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();

            // Загружаем жанры в комбобокс
            var genres = Core.Context.Genres.ToList();
            genres.Insert(0, new Genre { Name = "Все жанры" }); // Добавляем вариант для сброса фильтра
            CbGenre.ItemsSource = genres;
            CbGenre.SelectedIndex = 0;

            UpdateData();
        }

        private void UpdateData()
        {
            // Загружаем книги вместе с авторами и жанрами
            var query = Core.Context.Books
                .Include(b => b.User) // Подгружаем автора
                .Include(b => b.Genres) // Подгружаем жанры
                .Where(b => !b.IsFrozen)
                .AsQueryable();

            // Поиск (Title или Имя автора)
            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                string search = TbSearch.Text.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(search) ||
                                         b.User.DisplayName.ToLower().Contains(search));
            }

            // Фильтр по жанру
            if (CbGenre.SelectedIndex > 0)
            {
                int selectedGenreId = ((Genre)CbGenre.SelectedItem).Id;
                query = query.Where(b => b.Genres.Any(g => g.Id == selectedGenreId));
            }

            // Сортировка
            var list = query.ToList();
            if (CbSort.SelectedIndex == 0) list = list.OrderBy(x => x.Title).ToList();
            else if (CbSort.SelectedIndex == 1)
                list = list.OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0).ToList();

            LvBooks.ItemsSource = list;
        }
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int bookId = (int)button.Tag;
                // Переходим на страницу книги, передавая её ID
                NavigationService.Navigate(new BookDetailsPage(bookId));
            }
        }
        // Обработчики для обновления при вводе/выборе
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateData();
        private void CbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateData();
    }
}
