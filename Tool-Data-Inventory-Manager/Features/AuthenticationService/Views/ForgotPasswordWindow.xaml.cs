using System.Windows;

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
                MessageBox.Show("Please enter your email address.");
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show("No user found with this email address.");
                return;
            }

            int code = TemporaryCodeManager.GenerateCode(email);

            user.PasswordResetCode = code;
            user.PasswordResetRequestedAt = DateTime.UtcNow;

            _dbContext.SaveChanges();

            EmailSender.SendTemporaryCode(email, code.ToString());

            MessageBox.Show("Temporary code has been sent to your email.");
        }


        private void btn_VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text.Trim();

            var user = _dbContext.Users.SingleOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show("Email address not found.");
                return;
            }

            if (!int.TryParse(tb_Code.Text.Trim(), out int codeInput))
            {
                MessageBox.Show("Invalid code format.");
                return;
            }

            if (user.PasswordResetCode != codeInput)
            {
                MessageBox.Show("Invalid code.");
                return;
            }

            if (user.PasswordResetRequestedAt == null || user.PasswordResetRequestedAt.Value.AddMinutes(10) < DateTime.UtcNow)
            {
                MessageBox.Show("The code has expired. Please request a new password reset.");
                return;
            }

            MessageBox.Show("Code verified! Please set a new password.");
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
    }
}
