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
    public class HomeDashboardService : IHomeDashboardService
    {
        private readonly IHomeDashboardRepo _homeDashboardRepo;

        public HomeDashboardService(IHomeDashboardRepo homeDashboardRepo)
        {
            _homeDashboardRepo = homeDashboardRepo;
        }

        public async Task<List<DashboardFeaturePanel>> GetAllFeaturesAsync() =>
            await _homeDashboardRepo.GetAllFeaturesAsync();

        public async Task<bool> AddFeatureAsync(DashboardFeaturePanel feature) =>
            await _homeDashboardRepo.AddFeatureAsync(feature);

        public async Task<bool> UpdateFeatureAsync(DashboardFeaturePanel feature) =>
            await _homeDashboardRepo.UpdateFeatureAsync(feature);

        public async Task<bool> DeleteFeatureAsync(Guid id) =>
            await _homeDashboardRepo.DeleteFeatureAsync(id);
    }
}
