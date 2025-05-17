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
    public class HomeDashboardRepo : IHomeDashboardRepo
    {
        private readonly InventoryServiceDbContext _rmsServicedb;

        public HomeDashboardRepo(InventoryServiceDbContext context)
        {
            _rmsServicedb = context;
        }

        public async Task<List<DashboardFeaturePanel>> GetAllFeaturesAsync()
        {
            return await _rmsServicedb.FeaturePanels.ToListAsync();
        }

        public async Task<DashboardFeaturePanel> GetFeatureByIdAsync(Guid id)
        {
            return await _rmsServicedb.FeaturePanels.FindAsync(id);
        }

        public async Task<DashboardFeaturePanel> GetFeatureByKeyAsync(string viewKey)
        {
            return await _rmsServicedb.FeaturePanels.FirstOrDefaultAsync(x => x.FeatureViewKey == viewKey);
        }

        public async Task<bool> AddFeatureAsync(DashboardFeaturePanel feature)
        {
            if (await GetFeatureByKeyAsync(feature.FeatureViewKey) != null)
                return false;

            feature.FeatureId = Guid.NewGuid();
            _rmsServicedb.FeaturePanels.Add(feature);
            await _rmsServicedb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateFeatureAsync(DashboardFeaturePanel feature)
        {
            var existing = await GetFeatureByIdAsync(feature.FeatureId);
            if (existing == null)
                return false;

            existing.FeatureName = feature.FeatureName;
            existing.FeatureViewKey = feature.FeatureViewKey;
            _rmsServicedb.FeaturePanels.Update(existing);
            await _rmsServicedb.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFeatureAsync(Guid id)
        {
            var feature = await GetFeatureByIdAsync(id);
            if (feature == null)
                return false;

            _rmsServicedb.FeaturePanels.Remove(feature);
            await _rmsServicedb.SaveChangesAsync();
            return true;
        }
    }
}
