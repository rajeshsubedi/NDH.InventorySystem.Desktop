using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI.FeatureDashboard
{
    public partial class StockManagement : UserControl
    {
        private readonly ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> FilteredCategories => _categories;
        public ObservableCollection<UnitDetail> PrimaryUnits { get; set; } = new();
        public ObservableCollection<UnitDetail> SecondaryUnits { get; set; } = new();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new();

        private readonly IAddItemCategoryService _categoryService;
        private readonly IStockManagementService     _stockService;
        private StackPanel _previousActionIcons = null;
        private Category _selectedCategory;
        public string SelectedPrimaryUnit { get; set; }
        public string SelectedSecondaryUnit { get; set; }
        public string SelectedSupplier { get; set; }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                SelectedCategoryTextBlock.Text = _selectedCategory?.DisplayName ?? string.Empty;
            }
        }
        public StockManagement()
        {
            InitializeComponent();
            _categoryService = App.ServiceProvider.GetRequiredService<IAddItemCategoryService>();
            _stockService = App.ServiceProvider.GetRequiredService<IStockManagementService>();
            this.DataContext = this;
            Loaded += StockManagement_Loaded;
        }
        private async void StockManagement_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();              // ✅ Runs first
            await LoadUnitsAndSuppliersAsync();       // ✅ Runs only after first completes
        }

        private async Task LoadCategoriesAsync()
        {
            var allCategories = await _categoryService.GetAllCategoriesAsync();
            _categories.Clear();

            // Only load Parent Categories (Level 1 - PCG)
            var parentCategories = allCategories.Where(c => c.Level == 1).ToList();

            foreach (var parent in parentCategories)
            {
                // Only add the parent categories (PCG)
                _categories.Add(parent);
            }
        }

        private async Task LoadUnitsAndSuppliersAsync()
        {
            try
            {
                // Load Primary Units
                var primary = await _stockService.GetUnitsByTypeAsync("Primary");
                PrimaryUnits.Clear();
                foreach (var unit in primary)
                    PrimaryUnits.Add(unit);

                // Load Secondary Units (only AFTER primary units are done)
                var secondary = await _stockService.GetUnitsByTypeAsync("Secondary");
                SecondaryUnits.Clear();
                foreach (var unit in secondary)
                    SecondaryUnits.Add(unit);

                // Load Suppliers (only AFTER secondary units are done)
                var suppliers = await _stockService.GetAllAsync();
                Suppliers.Clear();
                foreach (var supplier in suppliers)
                    Suppliers.Add(supplier);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading units or suppliers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void AddPurchase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string quantityText = PurchaseQuantityTextBox?.Text?.Trim();
                string supplier = SelectedSupplier?.Trim();
                DateTime? purchaseDate = PurchaseDatePicker?.SelectedDate;

                if (string.IsNullOrWhiteSpace(quantityText) || !int.TryParse(quantityText, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid purchase quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(supplier))
                {
                    MessageBox.Show("Please enter the supplier name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (purchaseDate == null)
                {
                    MessageBox.Show("Please select a purchase date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var item = new StockItem
                {
                    ItemName = ItemNameTextBox.Text?.Trim(),
                    Category = SelectedCategoryTextBlock.Text?.Trim(),
                    PurchaseQuantity = quantity,
                    PrimaryUnit = SelectedPrimaryUnit,
                    SecondaryUnit = SelectedSecondaryUnit,
                    ConversionRate = decimal.TryParse(ConversionRateTextBox.Text, out var rate) ? rate : 0,
                    PurchasePrice = decimal.TryParse(PurchasePriceTextBox.Text, out var price) ? price : 0,
                    PurchaseDate = purchaseDate.Value,
                    Supplier = supplier
                };


                await _stockService.AddPurchaseAsync(item);

                MessageBox.Show("Stock purchase added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                PrimaryUnitComboBox.SelectedIndex = -1;
                SecondaryUnitComboBox.SelectedIndex = -1;
                SupplierComboBox.SelectedIndex = -1;
                SelectedPrimaryUnit = null;
                SelectedSecondaryUnit = null;
                SelectedSupplier = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Category_Clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is StackPanel panel)
            {
                var actionIcons = panel.FindName("ActionIcons") as StackPanel;

                if (_previousActionIcons != null && _previousActionIcons != actionIcons)
                {
                    _previousActionIcons.Visibility = Visibility.Collapsed;
                }

                if (actionIcons != null)
                {
                    actionIcons.Visibility = actionIcons.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    _previousActionIcons = actionIcons;
                }
            }
        }

        private async void AddParentCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = PromptForName("Enter parent category name:");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    await _categoryService.AddParentCategoryAsync(name);
                    await LoadCategoriesAsync();
                    MessageBox.Show("Parent category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                MessageBox.Show("A parent category with this name already exists. Please choose a different name.",
                                "Duplicate Category", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void AddSubCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CategoryTreeView.SelectedItem is Category selectedCategory)
                {
                    var subCategoryName = PromptForName($"Enter subcategory name for '{selectedCategory.DisplayName}':");
                    if (!string.IsNullOrWhiteSpace(subCategoryName))
                    {
                        await _categoryService.AddSubCategoryAsync(selectedCategory.CategoryId, subCategoryName);
                        await LoadCategoriesAsync();
                        MessageBox.Show("Subcategory added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a parent category before adding a subcategory.",
                                    "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                MessageBox.Show("A subcategory with this name already exists under the selected category. Please choose a different name.",
                                "Duplicate Subcategory", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                var newName = PromptForName($"Enter new name for '{selectedCategory.DisplayName}':");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    try
                    {
                        await _categoryService.EditCategoryAsync(selectedCategory.CategoryId, newName);
                        await LoadCategoriesAsync();

                        // Refresh the right panel with the updated category
                        SelectedCategory = FilteredCategories.FirstOrDefault(c => c.CategoryId == selectedCategory.CategoryId);
                        if (SelectedCategory != null)
                        {
                            SelectedCategoryTextBlock.Text = SelectedCategory.DisplayName;
                        }
                        else
                        {
                            SelectedCategoryTextBlock.Text = string.Empty;
                        }

                        MessageBox.Show("Category updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Edit Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedCategory.DisplayName}'?",
                                              "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _categoryService.DeleteCategoryAsync(selectedCategory.CategoryId);
                        await LoadCategoriesAsync();

                        // Clear the selected category
                        SelectedCategory = null;
                        SelectedCategoryTextBlock.Text = string.Empty;

                        // Manually refresh TreeView
                        CategoryTreeView.ItemsSource = null;
                        CategoryTreeView.ItemsSource = FilteredCategories;

                        MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private string PromptForName(string message)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(message, "Input");
        }

        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                SelectedCategory = selectedCategory;
            }
        }

     
        private void CategorySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = CategorySearchBox.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                // Show all categories if search is empty
                CategoryTreeView.ItemsSource = FilteredCategories;
                return;
            }

            // Filter categories based on search text
            var filtered = FilteredCategories
                .Where(c => c.DisplayName.ToLower().Contains(searchText) ||
                            c.SubCategories.Any(sub => sub.DisplayName.ToLower().Contains(searchText)))
                .ToList();

            CategoryTreeView.ItemsSource = filtered;
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            CategorySearchBox.Text = string.Empty;
        }

        private void AddItemImage_Click(object sender, RoutedEventArgs e)
        {
            //CategorySearchBox.Text = string.Empty;
        }

        private async void AddPrimaryUnit_Click(object sender, RoutedEventArgs e)
        {
            string unitName = Microsoft.VisualBasic.Interaction.InputBox("Enter new primary unit:", "Add Unit");
            if (!string.IsNullOrWhiteSpace(unitName))
            {
                await _stockService.AddUnitAsync(unitName.Trim(), "Primary");
                await LoadUnitsAndSuppliersAsync();
                MessageBox.Show("Unit added successfully!");
            }
        }

        private async void AddSecondaryUnit_Click(object sender, RoutedEventArgs e)
        {
            string unitName = Microsoft.VisualBasic.Interaction.InputBox("Enter new primary unit:", "Add Unit");
            if (!string.IsNullOrWhiteSpace(unitName))
            {
                await _stockService.AddUnitAsync(unitName.Trim(), "Secondary");
                await LoadUnitsAndSuppliersAsync();
                MessageBox.Show("Unit added successfully!");
            }
        }

        private async void DeletePrimaryUnit_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Unit ID to delete:", "Delete Unit");
            if (int.TryParse(input, out int id))
            {
                var result = MessageBox.Show($"Are you sure you want to delete unit with ID {id}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await _stockService.DeleteUnitAsync(id);
                    await LoadUnitsAndSuppliersAsync();
                    MessageBox.Show("Unit deleted successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric ID.");
            }
        }

    }
}
