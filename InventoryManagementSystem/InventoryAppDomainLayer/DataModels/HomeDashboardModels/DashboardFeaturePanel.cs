using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.HomeDashboardModels
{
    public class DashboardFeaturePanel
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureViewKey { get; set; } // e.g., "Dashboard", "Categories"
    }
}
