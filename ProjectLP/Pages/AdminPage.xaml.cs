using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        // Список ролей для привязки к ComboBox в DataGrid
        public List<Roles> AllRoles { get; set; }

        public AdminPage()
        {
            InitializeComponent();
            RefreshAll();
            this.DataContext = this;
        }

        private void RefreshAll()
        {
            // Загружаем справочник ролей
            AllRoles = Core.Context.Roles.ToList();

            // 1. Жалобы
            DgComplaints.ItemsSource = Core.Context.Complaints.Include(c => c.Users).ToList();

            // 2. Заявки на разморозку (Предполагается наличие таблицы UnfreezeRequests)
            DgUnfreezeRequests.ItemsSource = Core.Context.UnfreezeRequests.Where(r => r.Status == "Pending").ToList();

            // 3. Заявки на автора
            DgAuthorRequests.ItemsSource = Core.Context.RoleRequests.Where(r => r.Status == "Pending").Include(r => r.Users).ToList();

            // 4. Пользователи
            DgUsers.ItemsSource = Core.Context.Users.Include(u => u.Roles).ToList();

            // 5. Замороженные объекты
            LbFrozenBooks.ItemsSource = Core.Context.Books.Where(b => b.IsFrozen).ToList();
            LbFrozenUsers.ItemsSource = Core.Context.Users.Where(u => u.IsFrozen).ToList();
            LbFrozenReviews.ItemsSource = Core.Context.Reviews.Where(r => r.IsFrozen).ToList();
        }

        // --- ЛОГИКА ЖАЛОБ ---
        private void BtnDeleteObject_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaints)((Button)sender).DataContext;

            if (complaint.BookId != null)
                Core.Context.Books.Find(complaint.BookId).IsFrozen = true;
            else if (complaint.ReviewId != null)
                Core.Context.Reviews.Find(complaint.ReviewId).IsFrozen = true; // Теперь замораживаем, а не удаляем

            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            RefreshAll();
        }

        private void BtnDismissComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaints)((Button)sender).DataContext;
            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            RefreshAll();
        }

        // --- ЛОГИКА РАЗМОРОЗКИ ---
        private void BtnAcceptUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var request = (UnfreezeRequests)((Button)sender).DataContext;

            try
            {
                request.Status = "Accepted"; // Теперь поле существует!

                // Проверяем по новым полям TargetType или по старым BookId/UserId
                if (request.TargetType == "Book" || request.BookId != null)
                {
                    int id = request.TargetId ?? request.BookId.Value;
                    var book = Core.Context.Books.Find(id);
                    if (book != null) book.IsFrozen = false;
                }
                else if (request.TargetType == "User" || request.UserId != null)
                {
                    int id = request.TargetId ?? request.UserId.Value;
                    var user = Core.Context.Users.Find(id);
                    if (user != null) user.IsFrozen = false;
                }

                Core.Context.SaveChanges();
                RefreshAll();
                MessageBox.Show("Объект разморожен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void BtnDeclineUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var request = (UnfreezeRequests)((Button)sender).DataContext;
            request.Status = "Declined";
            Core.Context.SaveChanges();
            RefreshAll();
        }

        // --- ЛОГИКА РОЛЕЙ АВТОРА ---
        private void BtnAcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequests)((Button)sender).DataContext;
            request.Status = "Accepted";

            var user = Core.Context.Users.Find(request.UserId);
            if (user != null) user.RoleId = 2; // ID роли Автора

            Core.Context.SaveChanges();
            RefreshAll();
        }

        private void BtnDeclineAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequests)((Button)sender).DataContext;
            request.Status = "Declined";
            Core.Context.SaveChanges();
            RefreshAll();
        }

        // --- УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ ---
        private void TbUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DgUsers.ItemsSource = Core.Context.Users
                .Where(u => u.Login.Contains(TbUserSearch.Text))
                .Include(u => u.Roles).ToList();
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            var user = (Users)((Button)sender).DataContext;
            user.Password = "12345";
            Core.Context.SaveChanges();
            MessageBox.Show($"Пароль для {user.Login} сброшен");
        }

        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var user = cb.DataContext as Users;
            if (user != null && cb.SelectedValue != null)
            {
                user.RoleId = (int)cb.SelectedValue;
                Core.Context.SaveChanges();
            }
        }
    }
}
