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

namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly LoginRegisterViewModel _viewModel;
        public Login()
        {
            InitializeComponent();
            _viewModel = new LoginRegisterViewModel();
            DataContext = _viewModel;
        }
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = tb_Email.Text;
            string password = pb_password.Password;
            if(_viewModel.Login(email, password))
            {
                SelectMachineWindow smw = new SelectMachineWindow();
                this.Close();
                smw.Show();
            }
        }

        private void tb_notMember_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterViewModel rw = new RegisterViewModel();
            this.Close();
            rw.Show();
        }

        private void tb_forgotPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPasswordWindow fpw = new ForgotPasswordWindow();
            this.Close();
            fpw.Show();
        }
    }
}
