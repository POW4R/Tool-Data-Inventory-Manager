using System.Windows;

namespace Tool_Data_Inventory_Manager
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void btn_SendCode_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email address.");
                return;
            }

            string code = TemporaryCodeManager.GenerateCode(email);
            EmailSender.SendTemporaryCode(email, code);

            MessageBox.Show("Temporary code has been sent to your email.");
        }

        private void btn_VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text.Trim();
            string codeInput = tb_Code.Text.Trim();

            if (TemporaryCodeManager.ValidateCode(email, codeInput))
            {
                MessageBox.Show("Code verified! Please set a new password.");
                ResetPasswordWindow resetWindow = new ResetPasswordWindow(email);
                resetWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid or expired code.");
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            Login l = new Login();
            this.Close();
            l.Show();
        }
    }
}
