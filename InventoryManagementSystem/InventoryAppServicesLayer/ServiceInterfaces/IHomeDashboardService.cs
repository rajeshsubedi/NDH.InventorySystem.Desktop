using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;

namespace InventoryAppServicesLayer.ServiceInterfaces
{
    public interface IHomeDashboardService
    {
        Task<List<DashboardFeaturePanel>> GetAllFeaturesAsync();
        Task<bool> AddFeatureAsync(DashboardFeaturePanel feature);
        Task<bool> UpdateFeatureAsync(DashboardFeaturePanel feature);
        Task<bool> DeleteFeatureAsync(Guid id);

    }
}
