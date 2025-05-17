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
        Task AddCategoryAsync(Category category);
        Task AddSubCategoryAsync(int parentCategoryId, string subCategoryName);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
