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
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) || string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Заполните логин и пароль!");
                return;
            }

            if (PbPassword.Password != PbConfirm.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (Core.Context.Users.Any(u => u.Login == TbLogin.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует!");
                return;
            }

            try
            {
                var newUser = new User
                {
                    Login = TbLogin.Text,
                    Password = PbPassword.Password,
                    Email = TbEmail.Text,
                    RoleId = 3,
                    IsFrozen = false,
                    DisplayName = TbLogin.Text,

                };

                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();

                MessageBox.Show("Успех!");
                NavigationService.GoBack();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        MessageBox.Show($"Ошибка в поле {validationError.PropertyName}: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
