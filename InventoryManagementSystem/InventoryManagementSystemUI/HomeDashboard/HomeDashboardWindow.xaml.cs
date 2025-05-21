using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InventoryAppServicesLayer.ServiceInterfaces;
using InventoryManagementSystemUI.FeatureDashboard;


namespace InventoryManagementSystemUI.HomeDashboard
{
    public partial class HomeDashboardWindow : Window
    {
        private Button _activeButton; // ← Add this
        private readonly IHomeDashboardService _homeDashboardService;

      
        public HomeDashboardWindow(IHomeDashboardService homeDashboardService)
        {
            InitializeComponent();
            GetFeatureCollection();
        }

        // Event handler for button clicks in the sidebar
        private void FeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeButton != null)
            {
                _activeButton.Style = (Style)FindResource("NavButtonStyle");
            }

            var clickedButton = sender as Button;
            if (clickedButton != null)
            {
                clickedButton.Style = (Style)FindResource("ActiveSidebarButtonStyle");
                _activeButton = clickedButton;

                var feature = clickedButton.DataContext as Feature;
                if (feature != null)
                {
                    switch (feature.FeatureTitle)
                    {
                        case "Add New Item":
                            FeatureDetailsContent.Content = new AddNewItemDashboard();
                            break;
                        case "Stock Management":
                            FeatureDetailsContent.Content = new StockManagement();
                            break;
                        case "Dashboard":
                            FeatureDetailsContent.Content = new Dashboard();
                            break;
                        default:
                            FeatureDetailsContent.Content = new TextBlock
                            {
                                Text = $"Loaded: {feature.FeatureTitle}",
                                FontSize = 20,
                                Margin = new Thickness(20)
                            };
                            break;
                    }
                }
            }
        }


        public void GetFeatureCollection()
        {
            FeatureItemsControl.ItemsSource = new List<Feature>
                {
                    new Feature { FeatureTitle = "Dashboard", FeatureDetails = "Overview of inventory statistics and KPIs." },
                    new Feature { FeatureTitle = "Add New Item", FeatureDetails = "Add new inventory items with relevant details." },
                    new Feature { FeatureTitle = "Stock Management", FeatureDetails = "Track current stock levels and restock alerts." },
                    new Feature { FeatureTitle = "Sales Tracking", FeatureDetails = "Monitor items sold and manage order history." },
                    new Feature { FeatureTitle = "Purchase Orders", FeatureDetails = "Manage supplier orders and track delivery status." },
                    new Feature { FeatureTitle = "Low Stock Alerts", FeatureDetails = "Get notifications for items running low in stock." },
                    new Feature { FeatureTitle = "Reports", FeatureDetails = "Generate inventory, sales, and audit reports." },
                    new Feature { FeatureTitle = "Category Management", FeatureDetails = "Organize inventory into categories for better tracking." },
                    new Feature { FeatureTitle = "User Access Control", FeatureDetails = "Manage user roles and permissions." },
                    new Feature { FeatureTitle = "Settings", FeatureDetails = "Configure system preferences and business settings." }
                };
        }
    }

    

    // Simple class for binding feature data
    public class Feature
    {
        public string FeatureTitle { get; set; }
        public string FeatureDetails { get; set; }
    }
}
