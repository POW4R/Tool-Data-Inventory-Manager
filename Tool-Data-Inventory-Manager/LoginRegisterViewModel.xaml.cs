using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_Data_Inventory_Manager
{
    public partial class LoginRegisterViewModel : Window
    {
        private readonly LoginRegisterViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new LoginRegisterViewModel();
            DataContext = _viewModel;
        }
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
            await _viewModel.Register();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
            _viewModel.Login();
        }
    }
}
