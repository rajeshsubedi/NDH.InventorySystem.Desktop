﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppServicesLayer.ServiceInterfaces
{
    public interface IAddItemCategoryService
    {
        Task AddParentCategoryAsync(string name);
        Task AddSubCategoryAsync(int parentCategoryId, string name);
        Task<List<Category>> GetAllCategoriesAsync();
        Task EditCategoryAsync(int categoryId, string newName);
        Task DeleteCategoryAsync(int categoryId);
    }
}
