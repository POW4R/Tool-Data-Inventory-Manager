using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true
            };

            var columnOpts = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn("Username", SqlDbType.NVarChar, dataLength: 100),
                new SqlColumn("EventContext", SqlDbType.NVarChar, dataLength: 250)
            }
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.MSSqlServer(
                    connectionString: "Server=localhost\\SQLEXPRESS;Database=Inventory Manager;Trusted_Connection=True;Encrypt=False;",
                    sinkOptions: sinkOpts,
                    columnOptions: columnOpts
                )
                .Enrich.WithProperty("AppName", "InventoryManager")
                .CreateLogger();

            Log.Information("Alkalmazás elindult.");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Alkalmazás leáll.");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}