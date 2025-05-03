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
using Microsoft.CodeAnalysis;
using Tool_Data_Inventory_Manager.Features.InventoryManager.Views;

namespace Tool_Data_Inventory_Manager.Features.AuthenticationService.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AppDbContext _dbContext;
        public LoginWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
        }
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            StartWindow mw = new StartWindow();
            this.Close();
            mw.Show();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text;
            string password = pb_password.Password;
            if(Login(email, password))
            {
                WorkspaceWindow ww = new WorkspaceWindow();
                this.Close();
                ww.Show();
            }
        }
        private bool Login(string Email, string Password)
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

        private void tb_notMember_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            this.Close();
            rw.Show();
        }

        private void tb_forgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordWindow fpw = new ForgotPasswordWindow();
            this.Close();
            fpw.Show();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
