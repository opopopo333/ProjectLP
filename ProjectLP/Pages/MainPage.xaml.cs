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

            if (user == null) return;

            BtnAdmin.Visibility = Visibility.Collapsed;
            BtnAuthor.Visibility = Visibility.Collapsed;

            if (user.RoleId == 1)
            {
                BtnAdmin.Visibility = Visibility.Visible;
            }
            else if (user.RoleId == 2)
            {
                BtnAuthor.Visibility = Visibility.Visible;
            }

            BtnFrozen.Visibility = user.IsFrozen ? Visibility.Visible : Visibility.Collapsed;

            if (ActiveFrame.Content == null)
                ActiveFrame.Navigate(new CatalogPage());
        }


        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new CatalogPage());
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new ProfilePage());
        }

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

        private void BtnFrozen_Click(object sender, RoutedEventArgs e)
        {
            ActiveFrame.Navigate(new FrozenWarningPage());
        }

        // Кнопка выхода (если есть в XAML)
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }
    }
}
