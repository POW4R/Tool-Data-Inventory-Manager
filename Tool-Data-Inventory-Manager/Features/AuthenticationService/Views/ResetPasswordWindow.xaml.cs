using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Tool_Data_Inventory_Manager.Features.AuthenticationService.Views
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly string _email;
        private readonly AppDbContext _dbContext;

        public ResetPasswordWindow(string email)
        {
            InitializeComponent();
            _email = email;
            _dbContext = new AppDbContext();
        }

        private async void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = pb_NewPassword.Password.Trim();
            string confirmPassword = pb_ConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show((string)Application.Current.FindResource("PleaseFillAllPasswordFields"));
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show((string)Application.Current.FindResource("PasswordsNotMatch"));
                return;
            }

            await ChangePassword(_email, newPassword, confirmPassword);
            LoginWindow lw = new LoginWindow();
            this.Close();
            lw.Show();
        }
        public async Task ChangePassword(string Email, string NewPassword, string ConfirmNewPassword)
        {
            if (NewPassword != ConfirmNewPassword)
            {
                MessageBox.Show((string)Application.Current.FindResource("NewPasswordsNotMatch"));
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                MessageBox.Show((string)Application.Current.FindResource("FieldsRequierd"));
                return;
            }

            if (!PasswordManager.IsValidPassword(NewPassword))
            {
                MessageBox.Show((string)Application.Current.FindResource("IncorrectPasswordFormat"));
                return;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("EmailNotExist"));
                return;
            }

            string hashedNewPassword = PasswordManager.HashPassword(NewPassword);
            user.PasswordHash = hashedNewPassword;

            try
            {
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                String errorMessage = (string)Application.Current.FindResource("ErrorChangingPassword") + ex.Message;
                MessageBox.Show(errorMessage);
            }

            MessageBox.Show((string)Application.Current.FindResource("PasswordChangedSuccessfully"));
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

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
