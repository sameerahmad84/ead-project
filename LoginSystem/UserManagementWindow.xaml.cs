using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LoginSystem.Data;
using LoginSystem.Models;
using System.IO;

namespace LoginSystem
{
    public partial class UserManagementWindow : Window
    {
        public required ApplicationDbContext _context;
        public required User _currentUser;
        public required Window _previousWindow;
        private readonly string _logPath = "error_log.txt";

        private void LogError(string methodName, Exception ex)
        {
            try
            {
                string errorMessage = $"[{DateTime.Now}] Error in {methodName}:\n{ex.Message}\nStack Trace:\n{ex.StackTrace}\n\n";
                File.AppendAllText(_logPath, errorMessage);
                MessageBox.Show($"An error occurred: {ex.Message}\nCheck error_log.txt for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception logEx)
            {
                MessageBox.Show($"Failed to log error: {logEx.Message}\nOriginal error: {ex.Message}", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public UserManagementWindow(User currentUser, Window previousWindow)
        {
            try
            {
                InitializeComponent();
                _currentUser = currentUser;
                _previousWindow = previousWindow;

                // Security check - only allow super-admin to access this window
                if (currentUser.Role != "super-admin")
                {
                    MessageBox.Show("Access denied. Only super-admin users can access the User Management window.", 
                        "Access Denied", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                try
                {
                    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer("Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;")
                        .Options;
                    
                    _context = new ApplicationDbContext(options);
                    _context.Database.CanConnect(); // Test the connection

                    LoadUsers();
                }
                catch (Exception dbEx)
                {
                    LogError("Database Connection", dbEx);
                    MessageBox.Show("Failed to connect to the database. Please check the connection string and try again.", 
                        "Database Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                    this.Close();
                    _previousWindow.Show();
                }
            }
            catch (Exception ex)
            {
                LogError(nameof(UserManagementWindow), ex);
                if (_previousWindow != null)
                {
                    _previousWindow.Show();
                }
                this.Close();
            }
        }

        private void LoadUsers()
        {
            try
            {
                usersGrid.ItemsSource = _context.Users.ToList();
            }
            catch (Exception ex)
            {
                LogError(nameof(LoadUsers), ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _previousWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnBack_Click), ex);
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addUserWindow = new AddEditUserWindow(null, _context, this);
                this.Hide();
                if (addUserWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
                this.Show();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnAddUser_Click), ex);
                this.Show();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadUsers();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnRefresh_Click), ex);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is User user)
                {
                    var editUserWindow = new AddEditUserWindow(user, _context, this);
                    this.Hide();
                    if (editUserWindow.ShowDialog() == true)
                    {
                        LoadUsers();
                    }
                    this.Show();
                }
            }
            catch (Exception ex)
            {
                LogError(nameof(btnEdit_Click), ex);
                this.Show();
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is User user)
                {
                    var passwordWindow = new ChangePasswordWindow(user, _context, this);
                    this.Hide();
                    passwordWindow.ShowDialog();
                    this.Show();
                }
            }
            catch (Exception ex)
            {
                LogError(nameof(btnChangePassword_Click), ex);
                this.Show();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is User user)
                {
                    // Prevent deleting super-admin
                    if (user.Role == "super-admin")
                    {
                        MessageBox.Show("Cannot delete super-admin account!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Prevent self-deletion
                    if (user.UserId == _currentUser.UserId)
                    {
                        MessageBox.Show("Cannot delete your own account!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var result = MessageBox.Show(
                        $"Are you sure you want to delete user '{user.Username}'?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _context.Users.Remove(user);
                            _context.SaveChanges();
                            LoadUsers();
                            MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            LogError("Delete User Operation", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(nameof(btnDelete_Click), ex);
            }
        }
    }
} 