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
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI.FeatureDashboard.StockManagement
{
    /// <summary>
    /// Interaction logic for AddSupplierWindow.xaml
    /// </summary>
    public partial class AddSupplierWindow : Window
    {
        public Supplier NewSupplier { get; private set; }
        private readonly IStockManagementService _stockService;

        public AddSupplierWindow()
        {
            InitializeComponent();
            _stockService = App.ServiceProvider.GetRequiredService<IStockManagementService>();
        }

        private async void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Supplier name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var supplier = new Supplier
            {
                Name = name,
                ContactPerson = ContactPersonTextBox.Text.Trim(),
                PhoneNumber = PhoneTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                AddressLine1 = Address1TextBox.Text.Trim(),
                AddressLine2 = Address2TextBox.Text.Trim(),
                City = CityTextBox.Text.Trim(),
                State = StateTextBox.Text.Trim(),
                ZipCode = ZipCodeTextBox.Text.Trim(),
                Country = CountryTextBox.Text.Trim(),
                PAN = PanTextBox.Text.Trim(),
                GSTNumber = GstTextBox.Text.Trim(),
                TIN = TinTextBox.Text.Trim(),
                Notes = NotesTextBox.Text.Trim(),
                CreatedAt = DateTime.Now
            };

            var success = await _stockService.AddSupplierAsync(supplier);

            if (!success)
            {
                MessageBox.Show("A supplier with this name already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.NewSupplier = supplier;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Or Close() if not using ShowDialog
            this.Close();
        }

    }
}
