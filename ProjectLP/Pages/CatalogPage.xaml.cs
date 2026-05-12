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
            // Базовый запрос: только не замороженные книги
            var list = Core.Context.Books.Where(b => b.IsFrozen == false).ToList();

            // Поиск по названию или автору
            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                list = list.Where(b => b.Title.ToLower().Contains(TbSearch.Text.ToLower()) ||
                                       b.User.DisplayName.ToLower().Contains(TbSearch.Text.ToLower())).ToList();
            }

            // Фильтрация по жанру
            if (CbGenre.SelectedIndex > 0)
            {
                var selectedGenre = CbGenre.SelectedItem as Genre;
                // Т.к. связь Many-to-Many, проверяем наличие жанра в коллекции BookGenres
                list = list.Where(b => b.Genres.Any(g => g.Id == selectedGenre.Id)).ToList();
            }

            // Сортировка
            if (CbSort.SelectedIndex == 0) // По имени
                list = list.OrderBy(b => b.Title).ToList();
            else if (CbSort.SelectedIndex == 1) // По рейтингу (если есть отзывы)
                list = list.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0).ToList();

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
