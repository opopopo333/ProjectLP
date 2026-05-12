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
    /// Interaction logic for FrozenWarningPage.xaml
    /// </summary>
    public partial class FrozenWarningPage : Page
    {
        public FrozenWarningPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = Core.CurrentUser;
        }

        private void BtnSendAppeal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbAppeal.Text))
            {
                MessageBox.Show("Пожалуйста, напишите текст апелляции.");
                return;
            }

            try
            {
                var request = new UnfreezeRequest
                {
                    UserId = Core.CurrentUser.Id,
                    Reason = TbAppeal.Text,
                    CreatedAt = DateTime.Now
                };

                Core.Context.UnfreezeRequests.Add(request);
                Core.Context.SaveChanges();

                MessageBox.Show("Ваша апелляция отправлена администратору!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: проверьте названия полей в БД. " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage());
        }
    }
}
