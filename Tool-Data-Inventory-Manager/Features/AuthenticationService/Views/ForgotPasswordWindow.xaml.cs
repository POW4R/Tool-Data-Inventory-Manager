using System.Windows;
using System.Windows.Input;

namespace Tool_Data_Inventory_Manager.Features.AuthenticationService.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly AppDbContext _dbContext;
        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
        }

        private void btn_SendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show((string)Application.Current.FindResource("PleaseEnterEmailAddress"));
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("NoUserFoundWithThisEmail"));
                return;
            }

            int code = TemporaryCodeManager.GenerateCode(email);

            user.PasswordResetCode = code;
            user.PasswordResetRequestedAt = DateTime.UtcNow;
            user.PasswordResetAttempts = 0;

            _dbContext.SaveChanges();

            EmailSender.SendTemporaryCode(email, code.ToString());

            MessageBox.Show((string)Application.Current.FindResource("TemporaryCodeHasBeenSentToEmail"));
        }


        private void btn_VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text.Trim();

            var user = _dbContext.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("EmailAddressNotFound"));
                return;
            }

            if (user.PasswordResetAttempts >= 3)
            {
                MessageBox.Show((string)Application.Current.FindResource("TooManyFailedAttemptsPleaseRequestNewPasswordReset"));
                return;
            }

            if (!int.TryParse(tb_Code.Text.Trim(), out int codeInput))
            {
                MessageBox.Show((string)Application.Current.FindResource("InvalidCodeFormat"));
                return;
            }
            

            if (user.PasswordResetCode != codeInput)
            {
                user.PasswordResetAttempts++;
                _dbContext.SaveChanges();
                MessageBox.Show((string)Application.Current.FindResource("InvalidCode"));
                return;
            }

            if (user.PasswordResetRequestedAt == null || user.PasswordResetRequestedAt.Value.AddMinutes(10) < DateTime.UtcNow)
            {
                MessageBox.Show((string)Application.Current.FindResource("TheCodeExpiredPleaseRequestNewPasswordReset"));
                return;
            }

            _dbContext.SaveChanges();

            MessageBox.Show((string)Application.Current.FindResource("CodeVerifiedPleaseSetANewPassword"));
            ResetPasswordWindow resetWindow = new ResetPasswordWindow(email);
            resetWindow.Show();
            this.Close();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            this.Close();
            lw.Show();
        }


        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
