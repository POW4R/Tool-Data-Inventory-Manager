using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Text.RegularExpressions;


namespace Tool_Data_Inventory_Manager
{
    public class LoginRegisterViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public LoginRegisterViewModel()
        {
            _dbContext = new AppDbContext();
            _dbContext.Database.EnsureCreated();
        }
        public async Task Register(string FirstName, string LastName, string Email, string Password, string ConfirmPassword)
        {
            if(Password != ConfirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek meg.");
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("A mezők kitöltése kötelező!");
                return;
            }

            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Hibás e-mail cím formátum.");
                return;
            }

            if(!PasswordManager.IsValidPassword(Password))
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
            var user = new User { FirstName = FirstName, LastName = LastName, Email=Email,PasswordHash = hashedPassword };

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

        /**
         * Bejelentkezés
         * @param Email
         * @param Password
         * @return true, ha a bejelentkezés sikeres, false, ha nem
         */
        public bool Login(string Email, string Password)
        {
            if (Password == null || Email == null)
            {
                MessageBox.Show("A mezők kitöltése kötelező!");
                return false;
            }
            string hashedPassword = PasswordManager.HashPassword(Password);
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                MessageBox.Show("Sikeres bejelentkezés!");
                return true;
            }
            else
            {
                MessageBox.Show("Hibás e-mail vagy jelszó.");
                return false;
            }
        }
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
