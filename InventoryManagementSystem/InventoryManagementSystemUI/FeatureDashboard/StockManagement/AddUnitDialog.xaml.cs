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

namespace InventoryManagementSystemUI.FeatureDashboard.StockManagement
{
    /// <summary>
    /// Interaction logic for AddUnitDialog.xaml
    /// </summary>
    public partial class AddUnitDialog : Window
    {
        public string UnitType { get; set; } = "Primary";
        public string UnitName => UnitNameTextBox.Text.Trim();
        public string LabelText => $"Enter new {UnitType.ToLower()} unit:";
        public AddUnitDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UnitName))
            {
                MessageBox.Show("Please enter a unit name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }
    }

}
