using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI.FeatureDashboard.AddNewItem
{
    /// <summary>
    /// Interaction logic for AddNewItemDashboard.xaml
    /// </summary>
    public partial class AddNewItemDashboard : UserControl
    {
        private readonly ObservableCollection<ProductCategory> _categories = new();
        public ObservableCollection<ProductCategory> FilteredCategories => _categories;

        private readonly IAddItemCategoryService _categoryService;
        private StackPanel _previousActionIcons = null;
        private ProductCategory _selectedCategory;
        public ProductCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                SelectedCategoryTextBlock.Text = _selectedCategory?.DisplayName ?? string.Empty;
            }
        }

        public AddNewItemDashboard()
        {
            InitializeComponent();
            _categoryService = App.ServiceProvider.GetRequiredService<IAddItemCategoryService>();
            this.DataContext = this;

            _ = LoadCategoriesAsync();
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
                if (CategoryTreeView.SelectedItem is ProductCategory selectedCategory)
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
            if (CategoryTreeView.SelectedItem is ProductCategory selectedCategory)
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
            if (CategoryTreeView.SelectedItem is ProductCategory selectedCategory)
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
            if (CategoryTreeView.SelectedItem is ProductCategory selectedCategory)
            {
                SelectedCategory = selectedCategory;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic Details
                string itemName = ItemNameTextBox?.Text?.Trim() ?? "";
                string category = SelectedCategoryTextBlock?.Text?.Trim() ?? "";
                bool isProduct = (FindName("ItemType_Product") as RadioButton)?.IsChecked == true;
                bool isService = (FindName("ItemType_Service") as RadioButton)?.IsChecked == true;

                // Stock Details
                string openingStockText = OpeningStockTextBox?.Text?.Trim() ?? "";
                string unit = UnitComboBox?.Text ?? "";
                string salesPriceText = SalesPriceTextBox?.Text?.Trim() ?? "";
                bool lowStockAlert = LowStockAlertToggle?.IsChecked == true;

                // Additional Details
                string itemCode = ItemCodeTextBox?.Text?.Trim() ?? "";
                string hsCode = HSCodeTextBox?.Text?.Trim() ?? "";
                string description = DescriptionTextBox?.Text?.Trim() ?? "";



                // Basic Validation
                if (string.IsNullOrEmpty(itemName))
                {
                    MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(category))
                {
                    MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //if (!isProduct && !isService)
                //{
                //    MessageBox.Show("Please select an item type (Product or Service).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    return;
                //}

                // Stock Details Validation (Optional for Service)
                if (isProduct)
                {
                    if (string.IsNullOrEmpty(openingStockText) || !int.TryParse(openingStockText, out int openingStock) || openingStock < 0)
                    {
                        MessageBox.Show("Please enter a valid opening stock (positive number).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.IsNullOrEmpty(unit))
                    {
                        MessageBox.Show("Please select a unit for stock.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!string.IsNullOrEmpty(salesPriceText) && !decimal.TryParse(salesPriceText, out decimal salesPrice))
                    {
                        MessageBox.Show("Sales Price must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }


                }

                // Display Success Message
                MessageBox.Show($"Item '{itemName}' added successfully!\nCategory: {category}\nType: {(isProduct ? "Product" : "Service")}",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear Form Fields
                ItemNameTextBox.Text = "";
                SelectedCategoryTextBlock.Text = "";
                OpeningStockTextBox.Text = "";
                UnitComboBox.SelectedIndex = 0;
                SalesPriceTextBox.Text = "";
                ItemCodeTextBox.Text = "";
                HSCodeTextBox.Text = "";
                DescriptionTextBox.Text = "";
                LowStockAlertToggle.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    }


}
