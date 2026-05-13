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
        private Books _currentBook;

        public BookDetailsPage(int bookId)
        {
            InitializeComponent();
            _currentBook = Core.Context.Books.FirstOrDefault(b => b.Id == bookId);
            this.DataContext = _currentBook;
            UpdateRating();
            LoadReviews();
            if (Core.CurrentUser?.RoleId == 1)
            {
                BtnFreezeBook.Visibility = Visibility.Visible;
                BtnFreezeBook.Content = _currentBook.IsFrozen ? "Разморозить книгу" : "Заморозить книгу";
            }
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

            var review = new Reviews
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
                Core.Context.ReadingLists.Add(new ReadingLists
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

        private void BtnReportReview_Click(object sender, RoutedEventArgs e)
        {
            var review = (Reviews)((Button)sender).DataContext;
            Core.Context.Complaints.Add(new Complaints
            {
                UserId = Core.CurrentUser.Id,
                ReviewId = review.Id,
                Reason = "Жалоба на отзыв"
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на отзыв отправлена");
        }

        private void BtnComplaintBook_Click(object sender, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaints
            {
                UserId = Core.CurrentUser.Id,
                BookId = _currentBook.Id,
                Reason = "Жалоба на книгу"
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на книгу отправлена");
        }

        private void BtnComplaintAuthor_Click(object sender, RoutedEventArgs e)
        {
            Core.Context.Complaints.Add(new Complaints
            {
                UserId = Core.CurrentUser.Id,
                Reason = "Жалоба на автора: " + _currentBook.Users.Login
            });
            Core.Context.SaveChanges();
            MessageBox.Show("Жалоба на автора отправлена");
        }

        private void UpdateRating()
        {
            try
            {
                var reviews = Core.Context.Reviews.Where(r => r.BookId == _currentBook.Id).ToList();

                if (reviews.Count > 0)
                {

                    double average = reviews.Average(r => (double)r.Rating);

                    TxtRatingValue.Text = average.ToString("0.0");
                    TxtReviewCount.Text = $"({reviews.Count} отзывов)";

                }
                else
                {
                    TxtRatingValue.Text = "0.0";
                    TxtReviewCount.Text = "(нет отзывов)";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка рейтинга: " + ex.Message);
            }
        }

        // Заморозка книги
        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переключаем статус (если была true, станет false и наоборот)
                _currentBook.IsFrozen = !_currentBook.IsFrozen;

                Core.Context.SaveChanges();

                MessageBox.Show(_currentBook.IsFrozen ? "Книга заморожена" : "Книга разморожена");
                BtnFreezeBook.Content = _currentBook.IsFrozen ? "Разморозить книгу" : "Заморозить книгу";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
