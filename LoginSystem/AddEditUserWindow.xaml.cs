using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class AddEditUserWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly User? _userToEdit;
        private readonly bool _isEditMode;
        private readonly Window _previousWindow;

        public AddEditUserWindow(User? userToEdit, ApplicationDbContext context, Window previousWindow)
        {
            InitializeComponent();
            _context = context;
            _userToEdit = userToEdit;
            _isEditMode = userToEdit != null;
            _previousWindow = previousWindow;

            if (_isEditMode)
            {
                txtHeader.Text = "Edit User";
                LoadUserData();
                
                // Hide password fields in edit mode since password changes should be done through Change Password window
                passwordPanel.Visibility = Visibility.Collapsed;
                txtPasswordLabel.Visibility = Visibility.Collapsed;
                txtConfirmPasswordLabel.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Collapsed;
                txtConfirmPassword.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadUserData()
        {
            if (_userToEdit != null)
            {
                txtUsername.Text = _userToEdit.Username;
                txtEmail.Text = _userToEdit.Email;
                cmbRole.SelectedItem = cmbRole.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString()?.ToLower() == _userToEdit.Role.ToLower());
            }
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
                // Validation
                if (string.IsNullOrWhiteSpace(txtUsername?.Text))
                {
                    MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword?.Password))
                {
                    MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_isEditMode && txtPassword?.Password != txtConfirmPassword?.Password)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedRole = (cmbRole?.SelectedItem as ComboBoxItem)?.Content?.ToString()?.ToLower();
                if (string.IsNullOrWhiteSpace(selectedRole))
                {
                    MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if username exists (for new users or when username is changed)
                var currentUsername = txtUsername?.Text ?? "";
                if ((!_isEditMode || (_userToEdit != null && _userToEdit.Username != currentUsername)) &&
                    _context.Users.Any(u => u.Username == currentUsername))
                {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_isEditMode && _userToEdit != null)
                {
                    // Update existing user
                    _userToEdit.Username = txtUsername?.Text ?? _userToEdit.Username;
                    _userToEdit.Email = txtEmail?.Text ?? _userToEdit.Email;
                    _userToEdit.Role = selectedRole;
                    _context.Users.Update(_userToEdit);
                }
                else
                {
                    // Create new user
                    var newUser = new User
                    {
                        Username = txtUsername?.Text ?? "",
                        Password = txtPassword?.Password ?? "",
                        Email = txtEmail?.Text ?? "",
                        Role = selectedRole,
                        CreatedDate = DateTime.Now
                    };
                    _context.Users.Add(newUser);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 