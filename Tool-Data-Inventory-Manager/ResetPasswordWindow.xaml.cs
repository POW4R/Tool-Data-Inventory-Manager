using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace Tool_Data_Inventory_Manager
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
                MessageBox.Show("Please fill in both password fields.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // Jelszó módosítása az adatbázisban
            await ChangePassword(_email, newPassword, confirmPassword);

            this.Close();
        }
        public async Task ChangePassword(string Email, string NewPassword, string ConfirmNewPassword)
        {
            if (NewPassword != ConfirmNewPassword)
            {
                MessageBox.Show("Az új jelszavak nem egyeznek meg.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                MessageBox.Show("A mezők kitöltése kötelező!");
                return;
            }

            if (!PasswordManager.IsValidPassword(NewPassword))
            {
                MessageBox.Show("Az új jelszónak legalább 8 karakter hosszúnak kell lennie, és tartalmaznia kell nagybetűt, kisbetűt, számot és speciális karaktert.");
                return;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                MessageBox.Show("Ez az e-mail cím nem létezik.");
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
                MessageBox.Show($"Hiba történt a jelszó módosításakor: {ex.Message}");
            }

            MessageBox.Show("A jelszó sikeresen megváltozott!");
        }


        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
