using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppServicesLayer.ServiceInterfaces
{
    public interface IStockManagementService
    {
        Task AddPurchaseAsync(StockItem item);

        //for unitDetail
        Task<List<UnitDetail>> GetUnitsByTypeAsync(string type);
        Task AddUnitAsync(string name, string type);
        Task EditUnitAsync(int id, string newName);
        Task DeleteUnitAsync(int id);

        //for supplier
        Task<List<Supplier>> GetAllAsync();
        Task<bool> AddSupplierAsync(Supplier supplier);
        Task EditAsync(int id, string newName);
        Task DeleteAsync(int id);
    }
}
