using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppDataAccessLayer.Repositories.RepoInterfaces
{
    public interface IStockManagementRepo
    {
        Task AddAsync(StockPurchases item);
        Task<List<StockPurchases>> GetAllStockItemsAsync();
        Task<List<string>> GetAllItemNamesAsync();
        // Unit
        Task<List<UnitDetail>> GetUnitsByTypeAsync(string type);
        Task AddUnitAsync(UnitDetail unit);
        Task EditUnitAsync(UnitDetail unit);
        Task DeleteUnitAsync(int id);

        // Supplier
        Task<List<Supplier>> GetAllSuppliersAsync();
        Task<bool> AddSupplierAsync(Supplier supplier);
        Task EditSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id);


    }
}
