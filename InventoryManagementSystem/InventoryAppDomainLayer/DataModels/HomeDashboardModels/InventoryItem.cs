using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.HomeDashboardModels
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }

        public int CategoryId { get; set; } // ✅ Foreign Key
        public ProductCategory Category { get; set; }

        public string HSCode { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal SalesPrice { get; set; }
        public int OpeningStock { get; set; }
        public int CurrentStock { get; set; }
        public int LowStockAlertLevel { get; set; }
        public bool LowStockAlertEnabled { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }

}
