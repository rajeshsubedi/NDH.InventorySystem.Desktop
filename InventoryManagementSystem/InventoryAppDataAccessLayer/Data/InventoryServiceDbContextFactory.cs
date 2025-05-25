using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventoryAppDataAccessLayer.Data
{
    public class InventoryServiceDbContextFactory : IDesignTimeDbContextFactory<InventoryServiceDbContext>
    {
        public InventoryServiceDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\InventoryManagementSystemUI");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<InventoryServiceDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new InventoryServiceDbContext(optionsBuilder.Options);
        }
    }
}
