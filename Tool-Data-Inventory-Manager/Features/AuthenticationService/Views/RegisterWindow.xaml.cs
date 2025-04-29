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
using System.Windows.Shapes;

namespace Tool_Data_Inventory_Manager.Features.AuthenticationService.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly AppDbContext _dbContext;
        public RegisterWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            string firstName = tb_FirstName.Text;
            string lastName = tb_LastName.Text;
            string email = tb_RegEmail.Text;
            string password = pb_Password.Password;
            string confirmPassword = pb_ConfirmPassword.Password;
            Register(firstName, lastName, email, password, confirmPassword);
        }
        public async Task Register(string FirstName, string LastName, string Email, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek meg.");
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("A mezők kitöltése kötelező!");
                return;
            }

            if (!EmailValidator.IsValidEmail(Email))
            {
                MessageBox.Show("Hibás e-mail cím formátum.");
                return;
            }

            if (!PasswordManager.IsValidPassword(Password))
            {
                MessageBox.Show("A jelszónak legalább 8 karakter hosszúnak kell lennie, és tartalmaznia kell nagybetűt, kisbetűt, számot és speciális karaktert.");
                return;
            }

            if (_dbContext.Users.Any(u => u.Email == Email))
            {
                MessageBox.Show("Ez az e-mail cím már foglalt.");
                return;
            }

            string hashedPassword = PasswordManager.HashPassword(Password);
            var user = new User { FirstName = FirstName, LastName = LastName, Email = Email, PasswordHash = hashedPassword };

            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MessageBox.Show("Sikeres regisztráció!");
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            StartWindow mw = new StartWindow();
            this.Close();
            mw.Show();
        }
    }
}
