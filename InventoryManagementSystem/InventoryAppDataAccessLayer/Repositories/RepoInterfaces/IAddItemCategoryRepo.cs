using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppDataAccessLayer.Repositories.RepoInterfaces
{
    public interface IAddItemCategoryRepo
    {
        Task AddCategoryAsync(ProductCategory category);
        Task<ProductCategory> GetCategoryByNameAndLevelAsync(string name, int level);
        Task AddSubCategoryAsync(int parentCategoryId, string subCategoryName);
        Task<ProductCategory?> GetCategoryByIdAsync(int id);
        Task<List<ProductCategory>> GetAllCategoriesAsync();
        Task UpdateCategoryAsync(ProductCategory category);
        Task DeleteCategoryAsync(ProductCategory category);
        Task<List<ProductCategory>> GetAllCategoriesWithSubcategoriesAsync();
        Task<List<StockPurchases>> GetAllStockPurchasesAsync();
    }
}
