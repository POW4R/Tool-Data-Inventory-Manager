using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            RegisterViewModel rw = new RegisterViewModel();
            this.Close();
            rw.Show();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
            _viewModel.Login();
        }
    }
}