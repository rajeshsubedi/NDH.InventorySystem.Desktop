using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.HomeDashboardModels
{
    public class ProductCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int Level { get; set; }
        public string LevelAbbreviation { get; set; }
        public int? ParentCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        public ProductCategory Parent { get; set; }
        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
        public List<StockPurchases> StockItems { get; set; } = new();

        public string DisplayCategoryName => $"{Name} ({LevelAbbreviation})";
    }

}
