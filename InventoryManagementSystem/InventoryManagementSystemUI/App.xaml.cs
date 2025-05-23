using System.Configuration;
using System.Data;
using System.Windows;
using InventoryManagementSystemUI.HomeDashboard;
using InventoryManagementSystemUI.Login;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterAllServices();
            base.OnStartup(e);
            //var loginWindow = ServiceProvider.GetRequiredService<LoginDashboard>();
            var loginWindow = ServiceProvider.GetRequiredService<HomeDashboardWindow>();

            loginWindow.Show();
        }

        public void RegisterAllServices()
        {
            var services = new ServiceCollection();
            // Register services
            services.RegisterServices();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}


