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
        private Book _currentBook;

        public BookDetailsPage(int bookId)
        {
            InitializeComponent();
            _currentBook = Core.Context.Books.FirstOrDefault(b => b.Id == bookId);
            this.DataContext = _currentBook;

            LoadReviews();
        }

        private void LoadReviews()
        {
            // Обновляем список отзывов для этой книги
            IcReviews.ItemsSource = Core.Context.Reviews.Where(r => r.BookId == _currentBook.Id).ToList();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void BtnSendReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbReviewText.Text)) return;

            var review = new Review
            {
                UserId = Core.CurrentUser.Id,
                BookId = _currentBook.Id,
                Text = TbReviewText.Text,
                Rating = (int)SliderRating.Value,
                CreatedAt = DateTime.Now
            };

            Core.Context.Reviews.Add(review);
            Core.Context.SaveChanges();

            TbReviewText.Clear();
            LoadReviews();
            MessageBox.Show("Отзыв добавлен!");
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            // Можно просто показать MessageBox с Content или открыть новую страницу
            MessageBox.Show(_currentBook.Content, "Чтение: " + _currentBook.Title);
        }

        // Добавление книги в список (чтение, планы и т.д.)
        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            // Логику выбора конкретного списка (статуса) можно сделать через ContextMenu или простое окно
            // Для прототипа добавим в "Читаю" по умолчанию
            var existingRecord = Core.Context.ReadingLists
                .FirstOrDefault(rl => rl.UserId == Core.CurrentUser.Id && rl.BookId == _currentBook.Id);

            if (existingRecord == null)
            {
                Core.Context.ReadingLists.Add(new ReadingList
                {
                    UserId = Core.CurrentUser.Id,
                    BookId = _currentBook.Id,
                    Status = "Читаю"
                });
                Core.Context.SaveChanges();
                MessageBox.Show("Добавлено в список 'Читаю'");
            }
            else
            {
                MessageBox.Show("Книга уже есть в ваших списках");
            }
        }

        // Жалоба на отзыв
        private void BtnReportReview_Click(object sender, RoutedEventArgs e)
        {
            var review = (Review)((Button)sender).DataContext;
            Core.Context.Complaints.Add(new Complaint
            {
                UserId = Core.CurrentUser.Id,
                ReviewId = review.Id,
                Reason = "Жалоба на отзыв"
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на отзыв отправлена");
        }

        // Жалоба на книгу
        private void BtnComplaintBook_Click(object sender, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaint
            {
                UserId = Core.CurrentUser.Id,
                BookId = _currentBook.Id,
                Reason = "Жалоба на книгу"
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на книгу отправлена");
        }

        // Жалоба на автора
        private void BtnComplaintAuthor_Click(object sender, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaint
            {
                UserId = Core.CurrentUser.Id,
                Reason = "Жалоба на автора: " + _currentBook.User.Login
                // В таблице Complaints нет прямой связи с автором, поэтому пишем в Reason
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на автора отправлена");
        }
    }
}
