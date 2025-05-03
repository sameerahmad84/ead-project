using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class RegisterWindow : Window
    {
        public required ApplicationDbContext _context;

        public RegisterWindow()
        {
            InitializeComponent();
            
            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(
                        "Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;",
                        opt => opt.EnableRetryOnFailure())
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .Options;
                
                _context = new ApplicationDbContext(options);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? $"\n\nInner Error: {ex.InnerException.Message}" : "";
                MessageBox.Show($"Database connection error: {ex.Message}{innerMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = txtUsername.Text?.Trim();
                var password = txtPassword.Password?.Trim();
                var confirmPassword = txtConfirmPassword.Password?.Trim();
                var email = txtEmail.Text?.Trim();

                // Validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPassword.Focus();
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConfirmPassword.Password = "";
                    txtConfirmPassword.Focus();
                    return;
                }

                if (username.Length > 50)
                {
                    MessageBox.Show("Username must be 50 characters or less.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password.Length > 100)
                {
                    MessageBox.Show("Password must be 100 characters or less.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(email) && email.Length > 100)
                {
                    MessageBox.Show("Email must be 100 characters or less.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Username already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newUser = new User
                {
                    Username = username ?? "",
                    Password = password ?? "",
                    Email = email ?? "",
                    Role = "clerk", // Default role for new registrations
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                MessageBox.Show("Registration successful! Please login.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Close registration window and show login window
                var loginWindow = new LoginWindow()
                {
                    _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer("Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;")
                        .Options)
                };
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? $"\n\nInner Error: {ex.InnerException.Message}" : "";
                MessageBox.Show($"Registration error: {ex.Message}{innerMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow()
            {
                _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;")
                    .Options)
            };
            loginWindow.Show();
            this.Close();
        }
    }
} 