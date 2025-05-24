using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDataAccessLayer.Data;
using InventoryAppDataAccessLayer.Repositories.RepoImplementations;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppServicesLayer.AuthorizationFilter;
using InventoryAppServicesLayer.ServiceImplementations;
using InventoryAppServicesLayer.ServiceInterfaces;
using InventoryManagementSystemUI.FeatureDashboard;
using InventoryManagementSystemUI.HomeDashboard;
using InventoryManagementSystemUI.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserAuthenticationRepo, UserAuthenticationRepo>();
            services.AddScoped<IHomeDashboardRepo, HomeDashboardRepo>();
            services.AddScoped<IAddItemCategoryRepo, AddItemCategoryRepo>();
            services.AddScoped<IStockManagementRepo, StockManagementRepo>();




            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<IHomeDashboardService, HomeDashboardService>();
            services.AddScoped<IAddItemCategoryService, AddItemCategoryService>();
            services.AddScoped<IStockManagementService, StockManagementService>();



            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ILoggerConfigurator, LoggerConfigurator>();
            // Singleton if needed
            services.AddSingleton<JwtAuthorizationFilter>();

            // Register the LoginDashboard window
            services.AddSingleton<LoginDashboard>();
            services.AddSingleton<HomeDashboardWindow>();
            services.AddSingleton<AddSupplierWindow>();
            services.AddSingleton<AddUnitDialog>();





            // Add db configuration 
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDbContext<InventoryServiceDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
