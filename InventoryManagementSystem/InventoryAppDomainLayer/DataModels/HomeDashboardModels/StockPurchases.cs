using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.HomeDashboardModels
{
    public class StockPurchases
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; } // Could be a foreign key reference if you have Category table

        public int CategoryLevel { get; set; }
        public string CategoryLevelAbbv { get; set; }

        public int PurchaseQuantity { get; set; }
        public string PrimaryUnit { get; set; }
        public string SecondaryUnit { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Supplier { get; set; }
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

    }
    public class Supplier
    {
        public int SupplierId { get; set; }

        // Basic Information
        public string Name { get; set; }               // Required
        public string ContactPerson { get; set; }      // Optional, useful for communication

        // Contact Details
        public string PhoneNumber { get; set; }        // Required
        public string Email { get; set; }              // Optional
        public string AddressLine1 { get; set; }       // Required
        public string AddressLine2 { get; set; }       // Optional
        public string City { get; set; }               // Required
        public string State { get; set; }              // Required
        public string ZipCode { get; set; }            // Optional
        public string Country { get; set; }            // Optional

        // Tax & Legal
        public string PAN { get; set; }                // Permanent Account Number (for Indian GST/Tax)
        public string GSTNumber { get; set; }          // GSTIN (if applicable)
        public string TIN { get; set; }                // Taxpayer Identification Number (general use)

        // Meta
        public string Notes { get; set; }              // Any extra info
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class UnitDetail
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // "Primary" or "Secondary"
    }
}
