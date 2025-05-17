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
using InventoryAppDataAccessLayer.Data;
using InventoryAppDataAccessLayer.Repositories.RepoImplementations;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppDomainLayer.Exceptions;
using InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO;
using InventoryAppServicesLayer.ServiceImplementations;
using InventoryAppServicesLayer.ServiceInterfaces;
using InventoryManagementSystemUI.HomeDashboard;

namespace InventoryManagementSystemUI.Login
{
    /// <summary>
    /// Interaction logic for LoginDashboard.xaml
    /// </summary>
    public partial class LoginDashboard : Window
    {
        private readonly IUserAuthenticationService _authService;
        private readonly IHomeDashboardService _homeDashboardService;

        public LoginDashboard(IUserAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            var loginRequest = new LoginRequestDTO
            {
                UserEmail = username,
                Password = password
            };

            try
            {
                var response = await _authService.AuthenticateLogin(loginRequest);
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Open next window and close login
                var dashboard = new HomeDashboardWindow(_homeDashboardService);
                dashboard.Show();
                this.Close();
            }
            catch (UserUnauthenticatedException)
            {
                MessageBox.Show("Wrong email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
