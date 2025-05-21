using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Tool_Data_Inventory_Manager.Features.AuthenticationService
{
    internal class Register : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public Register()
        {
            _dbContext = new AppDbContext();
            _dbContext.Database.EnsureCreated();
        }


        public async Task Registering(string FirstName, string LastName, string Email, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                MessageBox.Show((string) Application.Current.FindResource("PasswordsNotMatch"));
                return;
            }
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show((string)Application.Current.FindResource("FieldsRequierd"));
                return;
            }

            if (!EmailValidator.IsValidEmail(Email))
            {
                MessageBox.Show((string)Application.Current.FindResource("IncorrectEmailFormat"));
                return;
            }

            if (!PasswordManager.IsValidPassword(Password))
            {
                MessageBox.Show((string)Application.Current.FindResource("IncorrectPasswordFormat"));
                return;
            }

            if (_dbContext.Users.Any(u => u.Email == Email))
            {
                MessageBox.Show((string)Application.Current.FindResource("EmailAddressAlreadyTaken"));
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
            MessageBox.Show((string)Application.Current.FindResource("SuccessfullRegistration"));
        }
    }
}
