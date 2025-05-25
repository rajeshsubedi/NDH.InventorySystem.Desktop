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
        public string ItemCode { get; set; } // Unique item identifier
        public string Category { get; set; } // Consider using foreign key to a Category table
        public string HSCode { get; set; } // For compliance, optional
        public string Description { get; set; }

        public string Unit { get; set; } // Primary unit
        public decimal SalesPrice { get; set; }

        public int OpeningStock { get; set; } // Usually from first purchase or manually added
        public int CurrentStock { get; set; } // Updated from stock purchase/sales

        public int LowStockAlertLevel { get; set; }
        public bool LowStockAlertEnabled { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
