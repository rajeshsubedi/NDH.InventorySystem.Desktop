using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppDataAccessLayer.Repositories.RepoInterfaces
{
    public interface IHomeDashboardRepo
    {
        Task<List<DashboardFeaturePanel>> GetAllFeaturesAsync();
        Task<DashboardFeaturePanel> GetFeatureByIdAsync(Guid id);
        Task<DashboardFeaturePanel> GetFeatureByKeyAsync(string viewKey);
        Task<bool> AddFeatureAsync(DashboardFeaturePanel feature);
        Task<bool> UpdateFeatureAsync(DashboardFeaturePanel feature);
        Task<bool> DeleteFeatureAsync(Guid id);
    }
}
