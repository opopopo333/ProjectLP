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
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            RefreshAll();
        }

        private void RefreshAll()
        {
            // 1. Жалобы
            DgComplaints.ItemsSource = Core.Context.Complaints.Include(c => c.User).ToList();

            // 2. Заявки на авторов (фильтруем только "В ожидании")
            DgAuthorRequests.ItemsSource = Core.Context.RoleRequests.Where(r => r.Status == "Pending").ToList();

            // 3. Пользователи
            DgUsers.ItemsSource = Core.Context.Users.Include(u => u.Role).ToList();

            // 4. Заморозка
            LbFrozenBooks.ItemsSource = Core.Context.Books.Where(b => b.IsFrozen).ToList();
            LbFrozenUsers.ItemsSource = Core.Context.Users.Where(u => u.IsFrozen).ToList();
        }

        // --- ЛОГИКА ЖАЛОБ ---
        private void BtnDismissComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaint)((Button)sender).DataContext;
            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            RefreshAll();
        }

        // --- ЛОГИКА АВТОРСТВА ---
        private void BtnAcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequest)((Button)sender).DataContext;
            request.Status = "Accepted";

            // Меняем роль пользователя на "Автор" (предположим ID автора = 2)
            var user = Core.Context.Users.Find(request.UserId);
            user.RoleId = 2;

            Core.Context.SaveChanges();
            RefreshAll();
        }

        // --- ЛОГИКА ПОЛЬЗОВАТЕЛЕЙ ---
        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)((Button)sender).DataContext;
            // Здесь можно вызвать простой InputBox или просто захардкодить для прототипа
            user.Password = "12345";
            Core.Context.SaveChanges();
            MessageBox.Show($"Пароль для {user.Login} сброшен на 12345");
        }

        private void TbUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DgUsers.ItemsSource = Core.Context.Users.Where(u => u.Login.Contains(TbUserSearch.Text)).ToList();
        }

        // Удаление объекта (книги или отзыва) по жалобе
        private void BtnDeleteObject_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaints)((Button)sender).DataContext;

            if (complaint.BookId != null)
            {
                var book = Core.Context.Books.Find(complaint.BookId);
                book.IsFrozen = true; // Вместо удаления лучше заморозить по заданию
            }
            else if (complaint.ReviewId != null)
            {
                var review = Core.Context.Reviews.Find(complaint.ReviewId);
                Core.Context.Reviews.Remove(review);
            }

            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            RefreshAll();
            MessageBox.Show("Объект обработан");
        }

        // Отказ в получении роли автора
        private void BtnDeclineAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequests)((Button)sender).DataContext;
            request.Status = "Declined";
            Core.Context.SaveChanges();
            RefreshAll();
        }

        // Смена роли через ComboBox в DataGrid
        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var user = cb.DataContext as Users;
            var selectedRole = cb.SelectedItem as Roles;

            if (user != null && selectedRole != null)
            {
                user.RoleId = selectedRole.Id;
                Core.Context.SaveChanges();
            }
        }
    }
}
