using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDataAccessLayer.Data;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppDataAccessLayer.Repositories.RepoImplementations
{
    public class StockManagementRepo : IStockManagementRepo
    {
        private readonly InventoryServiceDbContext _context;

        public StockManagementRepo(InventoryServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StockPurchases item)
        {
            _context.StockPurchases.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StockPurchases>> GetAllStockItemsAsync()
        {
            return await _context.StockPurchases.ToListAsync();
        }
        public async Task<List<string>> GetAllItemNamesAsync()
        {
            return await _context.StockPurchases
                                 .Select(i => i.ItemName)
                                 .Distinct()
                                 .ToListAsync();
        }

        // Unit
        public async Task<List<UnitDetail>> GetUnitsByTypeAsync(string type)
        {
            return await _context.UnitDetails
                .Where(u => u.Type.ToLower() == type.ToLower())
                .ToListAsync();
        }

        public async Task AddUnitAsync(UnitDetail unit)
        {
            _context.UnitDetails.Add(unit);
            await _context.SaveChangesAsync();
        }

        public async Task EditUnitAsync(UnitDetail unit)
        {
            _context.UnitDetails.Update(unit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUnitAsync(int id)
        {
            var unit = await _context.UnitDetails.FindAsync(id);
            if (unit != null)
            {
                _context.UnitDetails.Remove(unit);
                await _context.SaveChangesAsync();
            }
        }

        // Supplier
        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            // Check if a supplier with the same name exists (case-insensitive)
            bool exists = await _context.Suppliers
                .AnyAsync(s => s.Name.ToLower() == supplier.Name.ToLower());

            if (exists)
                return false;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task EditSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }



    }
}
