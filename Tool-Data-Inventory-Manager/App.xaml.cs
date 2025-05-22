using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using DotNetEnv;


namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Env.Load();

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
                    connectionString: Environment.GetEnvironmentVariable("SQL_SERVER"),
                    sinkOptions: sinkOpts,
                    columnOptions: columnOpts
                )
                .Enrich.WithProperty("AppName", "InventoryManager")
                .CreateLogger();

            
            base.OnStartup(e);
            
            Log.Information("Alkalmazás elindult.");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Alkalmazás leáll.");
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        public static void ChangeLanguage(string languageCode)
        {
            var dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "hu":
                    dict.Source = new Uri("Resources/Localization/Strings.hu.xaml", UriKind.Relative);
                    break;
                case "de":
                    dict.Source = new Uri("Resources/Localization/Strings.de.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Resources/Localization/Strings.en.xaml", UriKind.Relative);
                    break;
            }

            var oldDict = Application.Current.Resources.MergedDictionaries
                            .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources"));

            if (oldDict != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }

}