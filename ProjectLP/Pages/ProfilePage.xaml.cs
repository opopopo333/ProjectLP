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
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = Core.CurrentUser;

            TxtDisplayName.Text = user.DisplayName;
            TxtLogin.Text = user.Login;
            TxtEmail.Text = user.Email;
            TxtRole.Text = user.Role?.Name ?? "Не определена";

            if (user.Role?.Name == "Пользователь")
            {
                BtnRequestAuthor.Visibility = Visibility.Visible;
            }

            // Видимость блока заморозки
            if (user.IsFrozen)
            {
                BorderFrozen.Visibility = Visibility.Visible;
            }

            // Загрузка отзывов пользователя
            LoadMyReviews();
        }

        private void LoadMyReviews()
        {
            LvMyReviews.ItemsSource = Core.Context.Reviews
                .Include(r => r.Book) 
                .Where(r => r.UserId == Core.CurrentUser.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        private void BtnRequestAuthor_Click(object sender, RoutedEventArgs e)
        {
            var existingRequest = Core.Context.RoleRequests
                .FirstOrDefault(r => r.UserId == Core.CurrentUser.Id && r.Status == "Pending");

            if (existingRequest != null)
            {
                MessageBox.Show("Ваша заявка уже находится на рассмотрении.");
                return;
            }

            Core.Context.RoleRequests.Add(new RoleRequest
            {
                UserId = Core.CurrentUser.Id,
                CreatedAt = DateTime.Now,
                Status = "Pending"

            });

            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена администратору!");
        }

        // Оспорить заморозку
        private void BtnUnfreeze_Click(object sender, RoutedEventArgs e)
        {

            Core.Context.UnfreezeRequests.Add(new UnfreezeRequest
            {
                UserId = Core.CurrentUser.Id,
                Reason = "Пользователь просит разблокировки через профиль",
                CreatedAt = DateTime.Now
            });

            Core.Context.SaveChanges();
            MessageBox.Show("Запрос на разморозку отправлен!");
        }
    }
}
