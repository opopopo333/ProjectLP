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
    /// Interaction logic for BookDetailsPage.xaml
    /// </summary>
    public partial class BookDetailsPage : Page
    {
        private int _currentBookId;

        // Изменяем конструктор так, чтобы он принимал ID книги
        public BookDetailsPage(int bookId)
        {
            InitializeComponent();

            _currentBookId = bookId;
            LoadBookData();
        }

        private void LoadBookData()
        {
            // Ищем книгу в базе по пришедшему ID
            var currentBook = Core.Context.Books.FirstOrDefault(b => b.Id == _currentBookId);

            if (currentBook != null)
            {
                // Здесь будет привязка данных к элементам интерфейса
                // Например: this.DataContext = currentBook;
            }
        }
    }
}
