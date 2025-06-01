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
    public class AddItemCategoryRepo : IAddItemCategoryRepo
    {
        private readonly InventoryServiceDbContext _rmsServicedb;

        public AddItemCategoryRepo(InventoryServiceDbContext context)
        {
            _rmsServicedb = context;
        }

        // Add Category Method
        public async Task AddCategoryAsync(ProductCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Ensure that if ParentCategoryId is set, the parent exists
            if (category.ParentCategoryId.HasValue)
            {
                var parent = await _rmsServicedb.ProductCategory.FindAsync(category.ParentCategoryId);
                if (parent == null)
                    throw new InvalidOperationException("Parent category not found.");

                // Ensure the parent is not a new root category
                if (parent.Level == 0)
                    throw new InvalidOperationException("Cannot add a subcategory to a root category.");
            }

            _rmsServicedb.ProductCategory.Add(category);
            await _rmsServicedb.SaveChangesAsync();
        }

        // Repository Method for Getting Category by Name and Level
        // Repository Method for Getting Category by Name and Level
        public async Task<ProductCategory> GetCategoryByNameAndLevelAsync(string name, int level)
        {
            return await _rmsServicedb.ProductCategory
                .Where(c => EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CI_AS") == name && c.Level == level)
                .FirstOrDefaultAsync();
        }



        // Add SubCategory Method
        public async Task AddSubCategoryAsync(int parentCategoryId, string subCategoryName)
        {
            if (string.IsNullOrEmpty(subCategoryName))
                throw new ArgumentException("Subcategory name cannot be empty.", nameof(subCategoryName));

            // Ensure the parent category exists
            var parentCategory = await _rmsServicedb.ProductCategory
                .FirstOrDefaultAsync(c => c.CategoryId == parentCategoryId);

            if (parentCategory == null)
            {
                throw new ArgumentException("Parent category not found.", nameof(parentCategoryId));
            }

            // Create and add the subcategory
            var subCategory = new ProductCategory
            {
                Name = subCategoryName,
                Level = parentCategory.Level + 1, // Subcategories are at a higher level
                ParentCategoryId = parentCategory.CategoryId,
                CreatedAt = DateTime.UtcNow // Set the creation date
            };

            _rmsServicedb.ProductCategory.Add(subCategory);
            await _rmsServicedb.SaveChangesAsync();
        }

        // Get Category By Id
        public async Task<ProductCategory?> GetCategoryByIdAsync(int categoryId)
        {
            return await _rmsServicedb.ProductCategory
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        //public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        //{
        //    return await _rmsServicedb.ProductCategory
        //        .Include(c => c.SubCategories)
        //        .ToListAsync();
        //}
        //public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        //{
        //    return await _rmsServicedb.ProductCategory
        //        .Include(c => c.SubCategories)
        //        .ThenInclude(sc => sc.SubCategories)
        //        .Include(c => c.StockItems) // ✅ Eager load items
        //        .ToListAsync();
        //}

        public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        {
            var categories = await _rmsServicedb.ProductCategory
                .Include(c => c.SubCategories)
                .ThenInclude(sc => sc.SubCategories)
                .ToListAsync();

            var purchases = await _rmsServicedb.StockPurchases.ToListAsync();

            // Attach StockPurchases manually
            foreach (var cat in categories)
            {
                cat.StockItems = purchases
                    .Where(p => p.ProductCategoryId == cat.CategoryId)
                    .ToList();
            }

            return categories;
        }




        public async Task UpdateCategoryAsync(ProductCategory category)
        {
            _rmsServicedb.ProductCategory.Update(category);
            await _rmsServicedb.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(ProductCategory category)
        {
            _rmsServicedb.ProductCategory.Remove(category);
            await _rmsServicedb.SaveChangesAsync();
        }

        public async Task<List<ProductCategory>> GetAllCategoriesWithSubcategoriesAsync()
        {
            return await _rmsServicedb.ProductCategory
                .Include(c => c.SubCategories)
                .ThenInclude(sc => sc.SubCategories)
                .ToListAsync();
        }

        public async Task<List<StockPurchases>> GetAllStockPurchasesAsync()
        {
            return await _rmsServicedb.StockPurchases.ToListAsync();
        }
    }
}