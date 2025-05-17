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
    public class AddItemCategoryService : IAddItemCategoryService
    {
        private readonly IAddItemCategoryRepo _repository;

        public AddItemCategoryService(IAddItemCategoryRepo repository)
        {
            _repository = repository;
        }

        // Add a top-level (parent) category
        public async Task AddParentCategoryAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.", nameof(name));

            var newCategory = new Category
            {
                Name = name,
                Abbreviation = GenerateAbbreviation(name),
                Level = 1,
                ParentCategoryId = null,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddCategoryAsync(newCategory);
        }

        // Add a subcategory under a specific parent
        public async Task AddSubCategoryAsync(int parentCategoryId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Subcategory name cannot be empty.", nameof(name));

            var parent = await _repository.GetCategoryByIdAsync(parentCategoryId);
            if (parent == null)
                throw new InvalidOperationException("Parent category not found.");

            var subCategory = new Category
            {
                Name = name,
                Abbreviation = GenerateAbbreviation(name),
                Level = parent.Level + 1,
                ParentCategoryId = parent.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddCategoryAsync(subCategory);
        }

        // Generate a 3-letter abbreviation from the name
        private string GenerateAbbreviation(string name)
        {
            return new string(name
                .Where(char.IsLetter)
                .Take(3)
                .ToArray())
                .ToUpper();
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _repository.GetAllCategoriesAsync();
        }
    }
}
