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
        private string _username;
        private string _password;
        private readonly AppDbContext _dbContext;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
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
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("A felhasználónév és jelszó kitöltése kötelező!");
                return;
            }

            if (_dbContext.Users.Any(u => u.Username == Username))
            {
                MessageBox.Show("Ez a felhasználónév már foglalt.");
                return;
            }

            string hashedPassword = HashPassword(Password);
            var user = new User { Username = Username, PasswordHash = hashedPassword };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            MessageBox.Show("Sikeres regisztráció!");
        }

        public void Login()
        {
            string hashedPassword = HashPassword(Password);
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == Username && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                MessageBox.Show("Sikeres bejelentkezés!");
            }
            else
            {
                MessageBox.Show("Hibás felhasználónév vagy jelszó.");
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
