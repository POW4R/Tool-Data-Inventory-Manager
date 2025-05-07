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
        private readonly Register reg;
        public RegisterWindow()
        {
            InitializeComponent();
            reg = new Register();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            StartWindow mw = new StartWindow();
            this.Close();
            mw.Show();
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            string firstName = tb_FirstName.Text;
            string lastName = tb_LastName.Text;
            string email = tb_RegEmail.Text;
            string password = pb_Password.Password;
            string confirmPassword = pb_ConfirmPassword.Password;
            reg.Registering(firstName, lastName, email, password, confirmPassword);
            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Close();
        }
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tb_alreadyHaveAccount_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            this.Close();
            lw.Show();
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
