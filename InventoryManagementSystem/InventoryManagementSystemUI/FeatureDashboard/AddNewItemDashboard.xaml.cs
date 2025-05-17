using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InventoryAppDomainLayer.DataModels.HomeDashboardModels;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystemUI.FeatureDashboard
{
    public partial class AddNewItemDashboard : UserControl
    {
        private readonly ObservableCollection<Category> _categories = new();
        public ObservableCollection<Category> FilteredCategories => _categories;

        private readonly IAddItemCategoryService _categoryService;
        private StackPanel _previousActionIcons = null;

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
            foreach (var category in allCategories)
            {
                _categories.Add(category);
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
            var name = PromptForName("Enter parent category name:");
            if (!string.IsNullOrEmpty(name))
            {
                await _categoryService.AddParentCategoryAsync(name);
                await LoadCategoriesAsync();
            }
        }

        private async void AddSubCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                var subCategoryName = PromptForName($"Enter subcategory name for '{selectedCategory.DisplayName}':");
                if (!string.IsNullOrWhiteSpace(subCategoryName))
                {
                    await _categoryService.AddSubCategoryAsync(selectedCategory.CategoryId, subCategoryName);
                    await LoadCategoriesAsync();
                }
            }
        }

        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                var newName = PromptForName($"Enter new name for '{selectedCategory.DisplayName}':");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    //await _categoryService.EditCategoryAsync(selectedCategory.CategoryId, newName);
                    await LoadCategoriesAsync();
                }
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryTreeView.SelectedItem is Category selectedCategory)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedCategory.DisplayName}'?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    //await _categoryService.DeleteCategoryAsync(selectedCategory.CategoryId);
                    await LoadCategoriesAsync();
                }
            }
        }

        private string PromptForName(string message)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(message, "Input");
        }

        private void CategoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CategoryTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                selectedItem.Background = Brushes.LightBlue;
                selectedItem.Foreground = Brushes.White;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            string itemName = ItemNameTextBox.Text.Trim();
            string itemDescription = ItemDescriptionTextBox.Text.Trim();
            string quantityText = QuantityTextBox.Text.Trim();
            string priceText = PriceTextBox.Text.Trim();

            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(quantityText) || string.IsNullOrEmpty(priceText))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Quantity must be a positive number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price) || price < 0)
            {
                MessageBox.Show("Price must be a valid non-negative number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Item '{itemName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Clear form
            ItemNameTextBox.Text = "";
            ItemDescriptionTextBox.Text = "";
            QuantityTextBox.Text = "";
            PriceTextBox.Text = "";
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
            CategoryTreeView.ItemsSource = FilteredCategories;
        }

    }
}