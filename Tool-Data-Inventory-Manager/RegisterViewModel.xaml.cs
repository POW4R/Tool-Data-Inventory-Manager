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
    /// Interaction logic for RegisterViewModel.xaml
    /// </summary>
    public partial class RegisterViewModel : Window
    {
        private readonly LoginRegisterViewModel _viewModel;
        public RegisterViewModel()
        {
            InitializeComponent();
            _viewModel = new LoginRegisterViewModel();
            DataContext = _viewModel;
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            string firstName = tb_FirstName.Text;
            string lastName = tb_LastName.Text;
            string email = tb_RegEmail.Text;
            string password = pb_Password.Password;
            string confirmPassword = pb_ConfirmPassword.Password;
            _viewModel.Register(firstName, lastName, email, password, confirmPassword);
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }
    }
}
