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
        ProductDataGrid.ItemsSource = _allProducts;
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
        if (ProductDataGrid.SelectedItem is Product selectedProduct)
        {
            var editor = new ProductEditorWindow(selectedProduct, _context) { Owner = this };
            if (editor.ShowDialog() == true)
            {
                _context.SaveChanges();
                LoadProducts();
            }
        }
    }
    private void ToolSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = ToolSearchBox.Text.ToLower();
        string selectedField = (ToolFilterField.SelectedItem as ComboBoxItem)?.Content.ToString();

        var filtered = _allTools.Where(tool =>
        {
            switch (selectedField)
            {
                case "Name":
                    return tool.Name?.ToLower().Contains(search) == true;
                case "MaterialNumber":
                    return tool.MaterialNumber.ToString().Contains(search);
                case "MagPlace":
                    return tool.MagPlace.HasValue && tool.MagPlace.Value.ToString().Contains(search);
                default:
                    return true;
            }
        }).ToList();

        ToolDataGrid.ItemsSource = filtered;
    }

    private void ProductSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string search = ProductSearchBox.Text.ToLower();
        string selectedField = (ProductFilterField.SelectedItem as ComboBoxItem)?.Content.ToString();

        var filtered = _allProducts.Where(p =>
        {
            switch (selectedField)
            {
                case "Type":
                    return p.Type?.ToLower().Contains(search) == true;
                case "ProductNumber":
                    return p.ProductNumber.ToString().Contains(search);
                case "Tools":
                    return p.Tools.Any(t => t.Name.ToLower().Contains(search));
                default:
                    return true;
            }
        });

        ProductDataGrid.ItemsSource = filtered.Select(p => new
        {
            p.Id,
            p.ProductNumber,
            p.Type,
            Tools = string.Join(", ", p.Tools.Select(t => t.Name))
        }).ToList();
    }
    private void DeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        if (ProductDataGrid.SelectedItem is Product selectedProduct)
        {
            var result = MessageBox.Show(
                $"Biztosan törölni szeretnéd a(z) \"{selectedProduct.ProductNumber} - {selectedProduct.Type}\" terméket?",
                "Megerősítés",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _context.Products.Remove(selectedProduct);
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
            MessageBox.Show("Előbb válassz ki egy terméket a törléshez.", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Information);
            Log.Information("Delete attempt without selecting a product.");
        }
    }
    private void DeleteTool_Click(object sender, RoutedEventArgs e)
    {
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

            var result = MessageBox.Show(
                $"Biztosan törölni szeretnéd a(z) \"{selectedTool.Name}\" szerszámot?",
                "Megerősítés",
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
            MessageBox.Show("Előbb válassz ki egy szerszámot a törléshez.", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Information);
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
        if (MachineDataGrid.SelectedItem != null)
        {
            int selectedIndex = MachineDataGrid.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _allMachines.Count)
            {
                var selectedMachine = _allMachines[selectedIndex];

                Log.Information("DeleteMachineButton_Click triggered for machine: {MachineName} (ID: {MachineId})", selectedMachine.Name, selectedMachine.Id);

                var result = MessageBox.Show(
                    $"Biztosan törölni szeretnéd a(z) \"{selectedMachine.Name}\" gépet?",
                    "Gép törlése",
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
            MessageBox.Show("Előbb válassz ki egy gépet a törléshez.", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Information);
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
    private void MachineSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string keyword = MachineSearchBox.Text.ToLower();
        string selectedField = (MachineFilterField.SelectedItem as ComboBoxItem)?.Content.ToString();

        var filtered = _allMachines.Where(m =>
        {
            if (selectedField == "Name")
            {
                return m.Name.ToLower().Contains(keyword);
            }
            else if (selectedField == "ProductNumber")
            {
                return m.Products.Any(p => p.ProductNumber.ToString().Contains(keyword));
            }
            return true;
        })
        .Select(m => new
        {
            m.Id,
            m.Name,
            Products = string.Join(", ", m.Products.Select(p => p.ProductNumber))
        }).ToList();

        MachineDataGrid.ItemsSource = filtered;
    }

    private void btn_export_Click(object sender, RoutedEventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            FileName = "TDV-list",
            DefaultExt = ".xlsx",
            Filter = "Excel munkafüzet (*.xlsx)|*.xlsx"
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
                        worksheet.Cell(currentRow, startCol).Value = machine.Name;
                        worksheet.Cell(currentRow, startCol + 1).Value = product.ProductNumber;
                        worksheet.Cell(currentRow, startCol + 2).Value = tool.Name;
                        worksheet.Cell(currentRow, startCol + 3).Value = tool.MaterialNumber;
                        worksheet.Cell(currentRow, startCol + 4).Value = tool.MagPlace;
                        currentRow++;
                    }
                }
            }
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

        try
        {
            workbook.SaveAs(saveDialog.FileName);
            MessageBox.Show("Sikeres exportálás Excel fájlba!", "Export kész", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hiba történt a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void DisableSortingOnDataGrid(DataGrid dataGrid)
    {
        foreach (var column in dataGrid.Columns)
        {
            column.CanUserSort = false;
        }
    }

    private void ToolFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(ToolSearchBox != null)
            ToolSearchBox.Clear();
    }

    private void ProductFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductSearchBox != null)
            ProductSearchBox.Clear();
    }

    private void MachineFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MachineSearchBox != null)
            MachineSearchBox.Clear();
    }

    private void btn_logout_Click(object sender, RoutedEventArgs e)
    {
        var massageBoxResult = MessageBox.Show("Biztosan ki szeretnél lépni?", "Kijelentkezés", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
}