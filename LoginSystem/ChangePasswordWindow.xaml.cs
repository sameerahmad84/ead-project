using System;
using System.Windows;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly User _user;
        private readonly Window _previousWindow;

        public ChangePasswordWindow(User user, ApplicationDbContext context, Window previousWindow)
        {
            InitializeComponent();
            _context = context;
            _user = user;
            _previousWindow = previousWindow;
            Title = $"Change Password - {user.Username}";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow.Show();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtNewPassword.Password != txtConfirmNewPassword.Password)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtNewPassword.Password.Length > 100)
                {
                    MessageBox.Show("Password must be 100 characters or less.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _user.Password = txtNewPassword.Password;
                _context.Users.Update(_user);
                _context.SaveChanges();

                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 