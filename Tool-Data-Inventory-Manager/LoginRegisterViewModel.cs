using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;


namespace Tool_Data_Inventory_Manager
{
    public class LoginRegisterViewModel : INotifyPropertyChanged
    {
        private string _firstname;
        private string _lastname;
        private string _password;
        private string _email;
        private readonly AppDbContext _dbContext;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }
        public string FirstName
        {
            get => _firstname;
            set { _firstname = value; OnPropertyChanged(nameof(FirstName)); }
        }
        public string LastName
        {
            get => _lastname;
            set { _lastname = value; OnPropertyChanged(nameof(LastName)); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public LoginRegisterViewModel()
        {
            _dbContext = new AppDbContext();
            _dbContext.Database.EnsureCreated();
        }

        public async Task Register()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("A mezők kitöltése kötelező!");
                return;
            }

            if (_dbContext.Users.Any(u => u.Email == Email))
            {
                MessageBox.Show("Ez az e-mail cím már foglalt.");
                return;
            }

            string hashedPassword = HashPassword(Password);
            var user = new User { FirstName = FirstName, LastName = LastName, PasswordHash = hashedPassword };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            MessageBox.Show("Sikeres regisztráció!");
        }

        public void Login()
        {
            string hashedPassword = HashPassword(Password);
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                MessageBox.Show("Sikeres bejelentkezés!");
            }
            else
            {
                MessageBox.Show("Hibás e-mail vagy jelszó.");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
