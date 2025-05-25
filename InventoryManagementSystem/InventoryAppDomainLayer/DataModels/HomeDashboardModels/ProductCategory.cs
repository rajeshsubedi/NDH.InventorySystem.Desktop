using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.HomeDashboardModels
{
    public class ProductCategory
    {
        public int CategoryId { get; set; } // Primary Key
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int Level { get; set; }
        public string LevelAbbreviation { get; set; } // <-- Add this field
        public int? ParentCategoryId { get; set; } // Foreign Key to parent category
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ProductCategory Parent { get; set; }
        public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
        public string DisplayName => $"{Name} ({LevelAbbreviation})";

        //public string DisplayName
        //{
        //    get
        //    {
        //        string levelAbbreviation = Level switch
        //        {
        //            1 => "PCG",
        //            2 => "SCG",
        //            3 => "CAT",
        //            4 => "CHC",
        //            5 => "SCC",
        //            _ => $"LVL{Level}"
        //        };
        //        return $"{Name} ({levelAbbreviation})";
        //    }
        //}
    }
}
