using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using InventoryAppServicesLayer.ServiceInterfaces;

namespace InventoryAppServicesLayer.ServiceImplementations
{
    public class StockManagementService : IStockManagementService
    {
        private readonly IStockManagementRepo _stockRepository;

        public StockManagementService(IStockManagementRepo stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task AddPurchaseAsync(StockItem item)
        {
            await _stockRepository.AddAsync(item);
        }

        public async Task<List<StockItem>> GetAllStockItemsAsync()
        {
            return await _stockRepository.GetAllStockItemsAsync();
        }

        public async Task<List<string>> GetAllItemNamesAsync()
        {
            return await _stockRepository.GetAllItemNamesAsync();
        }

        // Unit Methods
        public async Task<List<UnitDetail>> GetUnitsByTypeAsync(string type)
        {
            return await _stockRepository.GetUnitsByTypeAsync(type);
        }

        public async Task AddUnitAsync(string name, string type)
        {
            var unit = new UnitDetail { Name = name, Type = type };
            await _stockRepository.AddUnitAsync(unit);
        }

        public async Task EditUnitAsync(int id, string newName)
        {
            var unit = new UnitDetail { UnitId = id, Name = newName };
            await _stockRepository.EditUnitAsync(unit);
        }

        public async Task DeleteUnitAsync(int id)
        {
            await _stockRepository.DeleteUnitAsync(id);
        }

        // Supplier Methods
        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _stockRepository.GetAllSuppliersAsync();
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {

            return await _stockRepository.AddSupplierAsync(supplier);
        }


        public async Task EditAsync(int id, string newName)
        {
            var supplier = new Supplier { SupplierId = id, Name = newName };
            await _stockRepository.EditSupplierAsync(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            await _stockRepository.DeleteSupplierAsync(id);
        }
    }
}
