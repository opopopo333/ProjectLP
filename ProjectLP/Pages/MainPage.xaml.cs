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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = Core.CurrentUser;

            // 1. Проверяем заморозку
            if (user.IsFrozen)
            {
                BtnFrozen.Visibility = Visibility.Visible;
            }

            // 2. Проверяем роль (в БД Roles: 1-Админ, 2-Автор, 3-Пользователь)
            // Используй имена или ID из твоей таблицы Roles
            if (user.Role.Name == "Администратор")
            {
                BtnAdmin.Visibility = Visibility.Visible;
            }

            if (user.Role.Name == "Автор")
            {
                BtnAuthor.Visibility = Visibility.Visible;
            }

            // По умолчанию открываем каталог
            ActiveFrame.Navigate(new CatalogPage());
        }

        // Обработчики кликов (пока просто навигация)
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => ActiveFrame.Navigate(new CatalogPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => ActiveFrame.Navigate(new ProfilePage());
        // ... остальные методы по аналогии
        

        private void BtnLists_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new ListsPage());
        }

        private void BtnAuthor_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new AuthorPage());
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new AdminPage());
        }

        // Тот самый метод, которого не хватало
        private void BtnFrozen_Click(object sender, RoutedEventArgs e)
        {
            // Обычно при клике на варнинг открываем страницу с деталями заморозки
            ActiveFrame.Navigate(new FrozenWarningPage());
        }


    }
}
