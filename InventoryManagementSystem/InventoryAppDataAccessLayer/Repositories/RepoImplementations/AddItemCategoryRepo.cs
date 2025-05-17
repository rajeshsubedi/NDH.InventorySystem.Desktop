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
        public async Task AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Make sure that Category name is not empty before saving
            if (string.IsNullOrEmpty(category.Name))
                throw new ArgumentException("Category name cannot be empty.", nameof(category));

            _rmsServicedb.Categories.Add(category);
            await _rmsServicedb.SaveChangesAsync();
        }

        // Add SubCategory Method
        public async Task AddSubCategoryAsync(int parentCategoryId, string subCategoryName)
        {
            if (string.IsNullOrEmpty(subCategoryName))
                throw new ArgumentException("Subcategory name cannot be empty.", nameof(subCategoryName));

            // Ensure the parent category exists
            var parentCategory = await _rmsServicedb.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == parentCategoryId);

            if (parentCategory == null)
            {
                throw new ArgumentException("Parent category not found.", nameof(parentCategoryId));
            }

            // Create and add the subcategory
            var subCategory = new Category
            {
                Name = subCategoryName,
                Level = parentCategory.Level + 1, // Subcategories are at a higher level
                ParentCategoryId = parentCategory.CategoryId,
                CreatedAt = DateTime.UtcNow // Set the creation date
            };

            _rmsServicedb.Categories.Add(subCategory);
            await _rmsServicedb.SaveChangesAsync();
        }

        // Get Category By Id
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _rmsServicedb.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _rmsServicedb.Categories
                .Include(c => c.SubCategories)
                .ToListAsync();
        }
    }
}