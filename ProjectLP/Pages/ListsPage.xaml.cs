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
    /// Interaction logic for ListsPage.xaml
    /// </summary>
    public partial class ListsPage : Page
    {
        public ListsPage()
        {
            InitializeComponent();

            // Загрузка жанров для фильтрации
            var genres = Core.Context.Genres.ToList();
            genres.Insert(0, new Genre { Name = "Все жанры" });
            CbGenre.ItemsSource = genres;
            CbGenre.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => UpdateData();
        private void UpdateData(object sender, TextChangedEventArgs e) => UpdateData();

        private void UpdateData(object sender = null, SelectionChangedEventArgs e = null)
        {
            if (LvBooks == null || TabLists == null || CbGenre == null || CbSort == null)
                return;

            var selectedTab = TabLists.SelectedItem as TabItem;
            if (selectedTab == null) return; 

            string currentStatus = selectedTab.Header.ToString();

            var query = Core.Context.ReadingLists
                .Include("Book")
                .Include("Book.User")
                .Where(rl => rl.UserId == Core.CurrentUser.Id && rl.Status == currentStatus)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                string search = TbSearch.Text.ToLower();
                query = query.Where(rl => rl.Book.Title.ToLower().Contains(search) ||
                                         rl.Book.User.DisplayName.ToLower().Contains(search));
            }
            
            if (CbGenre.SelectedIndex > 0)
            {
                int gId = ((Genre)CbGenre.SelectedItem).Id;
                query = query.Where(rl => rl.Book.Genres.Any(g => g.Id == gId));
            }

            var list = query.ToList();

            if (CbSort.SelectedIndex == 0) list = list.OrderBy(x => x.Book.Title).ToList();
            else if (CbSort.SelectedIndex == 1)
                list = list.OrderByDescending(x => x.Book.Reviews.Any() ? x.Book.Reviews.Average(r => r.Rating) : 0).ToList();

            LvBooks.ItemsSource = list;
        }
        private void CbChangeStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null && cb.IsDropDownOpen) 
            {
                int recordId = (int)cb.Tag;
                var selectedItem = cb.SelectedItem as ComboBoxItem;
                string newStatus = selectedItem.Content.ToString();

                var record = Core.Context.ReadingLists.Find(recordId);
                if (record != null)
                {
                    record.Status = newStatus;
                    Core.Context.SaveChanges();
                    UpdateData(); 
                }
            }
        }

        private void BtnOpenBook_Click(object sender, RoutedEventArgs e)
        {
            int bookId = (int)((Button)sender).Tag;
            NavigationService.Navigate(new BookDetailsPage(bookId));
        }
    }
}
