using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using Microsoft.Win32;
using Serilog;
using Tool_Data_Inventory_Manager.Features.AuthenticationService.Views;
using System.DirectoryServices;
using System.Data;
using Tool_Data_Inventory_Manager.Features.Utils;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class WorkspaceWindow : Window
{
    private readonly AppDbContext _context;
    private List<Tool> _allTools;
    private List<Product> _allProducts;
    private Machine _selectedMachine;
    private List<Machine> _allMachines;
    private readonly string _loggedInUserName;
    private List<SearchResult> _searchResults;
    public WorkspaceWindow(User user)
    {
        InitializeComponent();
        _context = new AppDbContext();
        _context.Database.Migrate();

        _loggedInUserName = user.LastName + " " + user.FirstName;
        tb_loggedInUserName.Text = _loggedInUserName;
        LoadTools();
        LoadProducts();
        LoadMachines();
    }

    private void LoadTools()
    {
        _allTools = _context.Tools.ToList();
        ToolDataGrid.ItemsSource = _allTools;
        DisableSortingOnDataGrid(ToolDataGrid);
    }

    private void LoadProducts()
    {
        _allProducts = _context.Products.Include(p => p.Tools).ToList();

        var viewModelList = _allProducts.Select(p => new ProductViewModel(p)).ToList();

        ProductDataGrid.ItemsSource = viewModelList;
        DisableSortingOnDataGrid(ProductDataGrid);
    }
    private void LoadMachines()
    {
        _allMachines = _context.Machines.Include(m => m.Products).ToList();

        MachineDataGrid.ItemsSource = _allMachines.Select(m => new
        {
            m.Id,
            m.Name,
            Products = string.Join(", ", m.Products.Select(p => p.ProductNumber))
        }).ToList();
        DisableSortingOnDataGrid(MachineDataGrid);
    }
    private void AddTool_Click(object sender, RoutedEventArgs e)
    {
        var createWindow = new ToolCreateWindow { Owner = this };
        if (createWindow.ShowDialog() == true)
        {
            var newTool = createWindow.NewTool;
            _context.Tools.Add(newTool);
            _context.SaveChanges();
            LoadTools();
            LoadProducts();
            Log.Information($"Új szerszám hozzáadva: {newTool.Name} ({newTool.MaterialNumber})");
        }
    }
    private void ToolDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ToolDataGrid.SelectedItem is Tool selectedTool)
        {
            var editor = new ToolEditorWindow(selectedTool) { Owner = this };
            if (editor.ShowDialog() == true)
            {
                _context.SaveChanges();
                LoadTools();
                LoadProducts();
            }
        }
    }

    private void AddProduct_Click(object sender, RoutedEventArgs e)
    {
        var createWindow = new ProductCreateWindow(_context) { Owner = this };
        if (createWindow.ShowDialog() == true)
        {
            var newProduct = createWindow.NewProduct;
            _context.Products.Add(newProduct);
            _context.SaveChanges();
            LoadProducts();
        }
    }
    private void ProductDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ProductDataGrid.SelectedItem is ProductViewModel selectedViewModel)
        {
            var editor = new ProductEditorWindow(selectedViewModel.Product, _context) { Owner = this };

            if (editor.ShowDialog() == true)
            {
                _context.SaveChanges();
                LoadProducts();
            }
        }
    }
    
    private void DeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        string format = (string)Application.Current.Resources["DeleteProductConfirmation"];
        string confirm = (string)Application.Current.Resources["Confirm"];
        string selectProduct = (string)Application.Current.Resources["SelectProduct"];
        string select = (string)Application.Current.Resources["Selection"];
        if (ProductDataGrid.SelectedItem is ProductViewModel selectedProduct)
        {
            string deleteProductMessage = string.Format(format, selectedProduct.ProductNumber, selectedProduct.Type);
            var result = MessageBox.Show(
                deleteProductMessage,
                confirm,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Products.Remove(selectedProduct.Product);
                _context.SaveChanges();
                LoadProducts();
                Log.Information("Product deleted: {ProductNumber} - {Type}", selectedProduct.ProductNumber, selectedProduct.Type);
            } else
            {
                Log.Information("User canceled product deletion: {ProductNumber} - {Type}", selectedProduct.ProductNumber, selectedProduct.Type);
            }
        }
        else
        {
            MessageBox.Show(selectProduct, select, MessageBoxButton.OK, MessageBoxImage.Information);
            Log.Information("Delete attempt without selecting a product.");
        }
    }
    private void DeleteTool_Click(object sender, RoutedEventArgs e)
    {
        string selectTool = (string)Application.Current.Resources["SelectTool"];
        string selection = (string)Application.Current.Resources["Selection"];
        if (ToolDataGrid.SelectedItem is Tool selectedTool)
        {
            Log.Information("DeleteTool_Click triggered for tool: {ToolName} (ID: {ToolId})", selectedTool.Name, selectedTool.Id);

            var productsWithTool = _context.Products
                .Include(p => p.Tools)
                .Where(p => p.Tools.Any(t => t.Id == selectedTool.Id))
                .ToList();

            if (productsWithTool.Any())
            {
                Log.Information("Tool '{ToolName}' is assigned to {Count} product(s). Showing warning dialog.", selectedTool.Name, productsWithTool.Count);
                var warningWindow = new ToolHasProductsWindow(productsWithTool);
                warningWindow.Owner = this;

                if (warningWindow.ShowDialog() != true || !warningWindow.Confirmed)
                {
                    Log.Information("User canceled deletion of tool: {ToolName}", selectedTool.Name);
                    return;
                }
                Log.Information("User confirmed detachment of tool: {ToolName} from all related products", selectedTool.Name);
            }
            /*
            string format = (string)Application.Current.Resources["DeleteToolBox"];
            string confirm = (string)Application.Current.Resources["Confirm"];
            string deleteTool = string.Format(format, selectedTool.Name);
            var result = MessageBox.Show(
                deleteTool,
                confirm,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            */
            var result = MessageBox.Show(
                (string)Application.Current.Resources["DeleteToolBox"],
                (string)Application.Current.Resources["Confirm"],
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Log.Information("User confirmed deletion of tool: {ToolName}", selectedTool.Name);
                foreach (var product in productsWithTool)
                {
                    product.Tools.Remove(selectedTool);
                    Log.Information("Tool '{ToolName}' detached from product {ProductNumber}", selectedTool.Name, product.ProductNumber);
                }

                _context.Tools.Remove(selectedTool);
                _context.SaveChanges();
                Log.Information("Tool '{ToolName}' deleted from database.", selectedTool.Name);

                LoadTools();
                LoadProducts();
            } else
            {
                Log.Information("User declined deletion of tool: {ToolName}", selectedTool.Name);
            }
        }
        else
        {
            MessageBox.Show(selectTool, selection, MessageBoxButton.OK, MessageBoxImage.Information);
            Log.Information("DeleteTool_Click invoked with no tool selected.");
        }
    }

    private void AddMachineButton_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new AddMachineWindow(_context) { Owner = this };
        if (addWindow.ShowDialog() == true)
        {
            LoadMachines();
        }
    }
    private void DeleteMachineButton_Click(object sender, RoutedEventArgs e)
    {
        string format = (string)Application.Current.Resources["DeleteMeachineBox"];
        string deleteMachine = (string)Application.Current.Resources["DeleteMachine"];
        string selectMachine = (string)Application.Current.Resources["SelectMachine"];
        string selection = (string)Application.Current.Resources["Selection"];
        if (MachineDataGrid.SelectedItem != null)
        {
            int selectedIndex = MachineDataGrid.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _allMachines.Count)
            {
                var selectedMachine = _allMachines[selectedIndex];

                Log.Information("DeleteMachineButton_Click triggered for machine: {MachineName} (ID: {MachineId})", selectedMachine.Name, selectedMachine.Id);
                string deleteMachineBox = string.Format(format, selectedMachine.Name);
                var result = MessageBox.Show(
                    deleteMachineBox,
                    deleteMachine,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Log.Information("User confirmed deletion of machine: {MachineName}", selectedMachine.Name);

                    _context.Machines.Remove(selectedMachine);
                    _context.SaveChanges();

                    Log.Information("Machine '{MachineName}' deleted from database.", selectedMachine.Name);

                    LoadMachines();
                } else
                {
                    Log.Information("User canceled deletion of machine: {MachineName}", selectedMachine.Name);
                }
            }
        }
        else
        {
            MessageBox.Show(selectMachine, selection, MessageBoxButton.OK, MessageBoxImage.Information);
            Log.Information("DeleteMachineButton_Click invoked with no machine selected.");
        }
    }
    private void MachineDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (MachineDataGrid.SelectedItem != null)
        {
            int selectedIndex = MachineDataGrid.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _allMachines.Count)
            {
                var selectedMachine = _allMachines[selectedIndex];
                var editor = new EditMachineWindow(_context, selectedMachine) { Owner = this };
                if (editor.ShowDialog() == true)
                {
                    LoadMachines();
                }
            }
        }
    }
    private void AddMachine_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new MachineAddWindow(_context) { Owner = this };
        if (addWindow.ShowDialog() == true)
        {
            LoadMachines();
        }
    }

    private void btn_export_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            FileName = "TDV-list",
            DefaultExt = ".csv",
            Filter = "CSV file (*.csv)|*.csv"
        };

        if (saveDialog.ShowDialog() != true)
            return;

        try
        {
            using (var writer = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
            {
                // 1. Üres sor
                writer.WriteLine();

                // 2. Fejléc pontosvesszőkkel
                writer.WriteLine(";MachineName;ProductNumber;ToolName;MaterialNumber;MagPlace");

                // 3. Adatsorok
                foreach (var machine in _allMachines)
                {
                    foreach (var product in machine.Products)
                    {
                        var productWithTools = _context.Products
                            .Include(p => p.Tools)
                            .FirstOrDefault(p => p.Id == product.Id);

                        if (productWithTools != null)
                        {
                            foreach (var tool in productWithTools.Tools)
                            {
                                string line = ";" + string.Join(";", new[]
                                {
                                EscapeForCsv(machine.Name),
                                EscapeForCsv(product.ProductNumber.ToString()),
                                EscapeForCsv(tool.Name),
                                EscapeForCsv(tool.MaterialNumber.ToString()),
                                EscapeForCsv(tool.MagPlace.ToString())
                            });
                                writer.WriteLine(line);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string errorMessage = (string)Application.Current.Resources["Error"];
            string format = (string)Application.Current.Resources["ErrorBox"];
            string errorBox = string.Format(format, ex.Message);
            MessageBox.Show(errorBox, errorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string EscapeForCsv(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        // Ha van pontosvessző, idézőjel vagy sortörés, akkor idézőjelek közé tesszük
        if (input.Contains(";") || input.Contains("\"") || input.Contains("\n"))
            return $"\"{input.Replace("\"", "\"\"")}\"";

        return input;
    }


    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = txtSearch.Text;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            SearchResultsDataGrid.ItemsSource = null;
            return;
        }

        var _searchResults = new List<SearchResoult>();

        foreach (var machine in _allMachines)
        {
            foreach (var product in machine.Products)
            {
                var productWithTools = _context.Products
                    .Include(p => p.Tools)
                    .FirstOrDefault(p => p.Id == product.Id);

                if (productWithTools != null)
                {
                    foreach (var tool in productWithTools.Tools)
                    {
                        if ((machine.Name != null && machine.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (product.ProductNumber != null && product.ProductNumber.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (tool.Name != null && tool.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (tool.MaterialNumber != null && tool.MaterialNumber.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)) || // Fixed: Convert MaterialNumber to string
                            (tool.MagPlace != null && tool.MagPlace.Value.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase))) // Fixed: Convert MagPlace to string
                        {
                            _searchResults.Add(new SearchResoult
                            {
                                MachineName = machine.Name,
                                ProductNumber = product.ProductNumber.ToString(),
                                ToolName = tool.Name,
                                MaterialNumber = tool.MaterialNumber.ToString() ?? string.Empty,
                                MagPlace = tool.MagPlace?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
        }

        SearchResultsDataGrid.ItemsSource = _searchResults;
    }
    private void DisableSortingOnDataGrid(DataGrid dataGrid)
    {
        foreach (var column in dataGrid.Columns)
        {
            column.CanUserSort = false;
        }
    }

    private void btn_logout_Click(object sender, RoutedEventArgs e)
    {
        string logoutBox = (string)Application.Current.Resources["LogOutBox"];
        string logOut = (string)Application.Current.Resources["LogOut"];
        var massageBoxResult = MessageBox.Show(logoutBox, logOut, MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (massageBoxResult == MessageBoxResult.Yes)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        else
        {
            return;
        }
    }

    //If needed to export the search results, its here. And also the button exists in the XAML.
    private void btn_export_search_Click(object sender, RoutedEventArgs e)
    {
        var _searchResults = new List<SearchResoult>();
        var saveDialog = new SaveFileDialog
        {
            FileName = "TDV-Search_Resoult",
            DefaultExt = ".csv",
            Filter = "Excel munkafüzet (*.csv)|*.csv"
        };

        if (saveDialog.ShowDialog() != true)
            return;

        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Export");

        int startRow = 8;
        int startCol = 2;

        // Fejlécek
        worksheet.Cell(startRow, startCol).Value = "MachineName";
        worksheet.Cell(startRow, startCol + 1).Value = "ProductNumber";
        worksheet.Cell(startRow, startCol + 2).Value = "ToolName";
        worksheet.Cell(startRow, startCol + 3).Value = "MaterialNumber";
        worksheet.Cell(startRow, startCol + 4).Value = "MagPlace";

        int currentRow = startRow + 1;
        foreach (var resoult in _searchResults)
        {
            worksheet.Cell(currentRow, startCol).Value = resoult.MachineName;
            worksheet.Cell(currentRow, startCol + 1).Value = resoult.ProductNumber;
            worksheet.Cell(currentRow, startCol + 2).Value = resoult.ToolName;
            worksheet.Cell(currentRow, startCol + 3).Value = resoult.MaterialNumber;
            worksheet.Cell(currentRow, startCol + 4).Value = resoult.MagPlace;
        }
        // Formázás
        var headerRange = worksheet.Range(startRow, startCol, startRow, startCol + 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

        var dataRange = worksheet.Range(startRow, startCol, currentRow - 1, startCol + 4);
        dataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

        worksheet.Columns().AdjustToContents();

        string exportExel = (string)Application.Current.Resources["ExportExcel"];
        try
        {
            workbook.SaveAs(saveDialog.FileName);
            string exportOk = (string)Application.Current.Resources["ExportOk"];
            MessageBox.Show(exportExel, exportOk, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            string errorMessage = (string)Application.Current.Resources["Error"];
            string format = (string)Application.Current.Resources["ErrorBox"];
            string errorBox = string.Format(format, ex.Message);
            MessageBox.Show(errorBox, errorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
