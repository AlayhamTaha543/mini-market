using System.Windows;
using MiniMarketApp.Data;

namespace MiniMarketApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var databaseContext = new DatabaseContext();
        DatabaseInitializer.Initialize(databaseContext);
    }
}
