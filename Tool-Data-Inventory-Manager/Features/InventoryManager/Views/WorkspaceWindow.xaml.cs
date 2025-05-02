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
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

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


    public WorkspaceWindow()
    {
        InitializeComponent();
        _context = new AppDbContext();
        _context.Database.Migrate();

        LoadTools();
        LoadProducts();
        LoadMachines();
    }

    private void LoadTools()
    {
        _allTools = _context.Tools.ToList();
        ToolDataGrid.ItemsSource = _allTools;
    }

    private void LoadProducts()
    {
        _allProducts = _context.Products.Include(p => p.Tools).ToList();
        ProductDataGrid.ItemsSource = _allProducts;
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
                case "Price":
                    return tool.Price.ToString().Contains(search);
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
            }
        }
        else
        {
            MessageBox.Show("Előbb válassz ki egy terméket a törléshez.", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    private void DeleteTool_Click(object sender, RoutedEventArgs e)
    {
        if (ToolDataGrid.SelectedItem is Tool selectedTool)
        {
            var productsWithTool = _context.Products
                .Include(p => p.Tools)
                .Where(p => p.Tools.Any(t => t.Id == selectedTool.Id))
                .ToList();

            if (productsWithTool.Any())
            {
                var warningWindow = new ToolHasProductsWindow(productsWithTool);
                warningWindow.Owner = this;

                if (warningWindow.ShowDialog() != true || !warningWindow.Confirmed)
                    return;
            }

            var result = MessageBox.Show(
                $"Biztosan törölni szeretnéd a(z) \"{selectedTool.Name}\" szerszámot?",
                "Megerősítés",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var product in productsWithTool)
                {
                    product.Tools.Remove(selectedTool);
                }

                _context.Tools.Remove(selectedTool);
                _context.SaveChanges();

                LoadTools();
                LoadProducts();
            }
        }
        else
        {
            MessageBox.Show("Előbb válassz ki egy szerszámot a törléshez.", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Information);
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
        var selectedItem = MachineDataGrid.SelectedItem;
        if (selectedItem != null)
        {
            var machine = selectedItem.GetType().GetProperty("Original")?.GetValue(selectedItem) as Machine;
            if (machine != null)
            {
                var result1 = MessageBox.Show(
                    $"Biztosan törölni szeretnéd a(z) '{machine.Name}' gépet?",
                    "Első megerősítés",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result1 == MessageBoxResult.Yes)
                {
                    if (machine.Products.Any())
                    {
                        var result2 = MessageBox.Show(
                            "Ez a gép termékekhez van rendelve. Törlés előtt el kell távolítani a kapcsolatot.\nFolytatod?",
                            "Figyelmeztetés",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result2 != MessageBoxResult.Yes)
                            return;

                        // Kapcsolatok eltávolítása
                        machine.Products.Clear();
                    }

                    _context.Machines.Remove(machine);
                    _context.SaveChanges();
                    LoadMachines();
                }
            }
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
}