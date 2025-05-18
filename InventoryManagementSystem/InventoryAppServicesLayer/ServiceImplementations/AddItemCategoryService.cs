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

            // Check for duplicate parent category name (only for Level 1)
            var existingCategory = await _repository.GetCategoryByNameAndLevelAsync(name, 1);
            if (existingCategory != null)
                throw new InvalidOperationException($"A parent category with the name '{name}' already exists.");

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

            // Enforce 4 level limit (PCG > SCG > CAT > CHC)
            if (parent.Level >= 4)
                throw new InvalidOperationException("You cannot add a subcategory beyond level 4.");

            // Check for duplicate subcategory name within the parent
            if (parent.SubCategories.Any(sub => sub.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A subcategory with the name '{name}' already exists under '{parent.Name}'.");

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

        public async Task EditCategoryAsync(int categoryId, string newName)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found.");

            category.Name = newName;
            category.Abbreviation = GenerateAbbreviation(newName);
            await _repository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _repository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found.");

            if (category.SubCategories.Any())
                throw new InvalidOperationException("Cannot delete a category with subcategories.");

            await _repository.DeleteCategoryAsync(category);
        }
    }
}
