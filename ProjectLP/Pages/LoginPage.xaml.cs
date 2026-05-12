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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text;
            string password = PbPassword.Password;

            // Ищем пользователя в БД
            var user = Core.Context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                // Сохраняем пользователя в глобальную переменную
                Core.CurrentUser = user;

                // Переходим на главную страницу (её создадим следующим шагом)
                NavigationService.Navigate(new MainWindow());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            // Пока оставим пустым или выведем сообщение
            MessageBox.Show("Переход к регистрации");
            // В будущем здесь будет: NavigationService.Navigate(new RegistrationPage());
        }


    }
}
