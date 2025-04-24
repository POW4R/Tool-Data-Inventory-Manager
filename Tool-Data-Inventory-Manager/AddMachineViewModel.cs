using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_Data_Inventory_Manager
{
    public class AddMachineViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public AddMachineViewModel()
        {
            _dbContext = new AppDbContext();
            _dbContext.Database.EnsureCreated();
        }

        public async Task AddMachine(int machine_number)
        {
            if (machine_number <= 0)
            {
                MessageBox.Show("A gép számának pozitívnak kell lennie.");
                return;
            }
            if (_dbContext.Machine_Products.Any(m => m.Machine_Number == machine_number))
            {
                MessageBox.Show("Ez a gép már létezik.");
                return;
            }
            var machine = new Machine_Product { Machine_Number = machine_number };
            _dbContext.Machine_Products.Add(machine);
            await _dbContext.SaveChangesAsync();
        }
    }
}
