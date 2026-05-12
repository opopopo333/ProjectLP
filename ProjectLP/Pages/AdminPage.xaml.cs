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
        public AdminPage()
        {
            InitializeComponent();
            RefreshAll();
        }

        private void RefreshAll()
        {

            DgComplaints.ItemsSource = Core.Context.Complaints.Include(c => c.User).ToList();

            DgAuthorRequests.ItemsSource = Core.Context.RoleRequests.Where(r => r.Status == "Pending").ToList();

            DgUsers.ItemsSource = Core.Context.Users.Include(u => u.Role).ToList();

            LbFrozenBooks.ItemsSource = Core.Context.Books.Where(b => b.IsFrozen).ToList();
            LbFrozenUsers.ItemsSource = Core.Context.Users.Where(u => u.IsFrozen).ToList();
        }

        private void BtnDismissComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaint)((Button)sender).DataContext;
            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            RefreshAll();
        }

        private void BtnAcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequest)((Button)sender).DataContext;
            request.Status = "Accepted";

            var user = Core.Context.Users.Find(request.UserId);
            user.RoleId = 2;

            Core.Context.SaveChanges();
            RefreshAll();
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            var user = (User)((Button)sender).DataContext;
            user.Password = "12345";
            Core.Context.SaveChanges();
            MessageBox.Show($"Пароль для {user.Login} сброшен на 12345");
        }

        private void TbUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            DgUsers.ItemsSource = Core.Context.Users.Where(u => u.Login.Contains(TbUserSearch.Text)).ToList();
        }
 
        private void BtnDeleteObject_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (Complaint)((Button)sender).DataContext;

            if (complaint.BookId != null)
            {
                var book = Core.Context.Books.Find(complaint.BookId);
                book.IsFrozen = true; 
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

        private void BtnDeclineAuthor_Click(object sender, RoutedEventArgs e)
        {
            var request = (RoleRequest)((Button)sender).DataContext;
            request.Status = "Declined";
            Core.Context.SaveChanges();
            RefreshAll();
        }
        private void CbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            var user = cb.DataContext as User;
            var selectedRole = cb.SelectedItem as Role;

            if (user != null && selectedRole != null)
            {
                user.RoleId = selectedRole.Id;
                Core.Context.SaveChanges();
            }
        }
    }
}
