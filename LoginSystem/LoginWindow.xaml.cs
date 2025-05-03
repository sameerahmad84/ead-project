using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class LoginWindow : Window
    {
        public required ApplicationDbContext _context;

        public LoginWindow()
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

                // Ensure database exists and is up to date
                _context.Database.Migrate();

                // Create default super-admin if not exists
                if (!_context.Users.Any(u => u.Role == "super-admin"))
                {
                    var superAdmin = new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Email = "admin@admin.com",
                        Role = "super-admin",
                        CreatedDate = DateTime.Now
                    };
                    _context.Users.Add(superAdmin);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? $"\n\nInner Error: {ex.InnerException.Message}" : "";
                MessageBox.Show($"Database connection error: {ex.Message}{innerMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = txtUsername.Text?.Trim();
                var password = txtPassword.Password?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = _context.Users.FirstOrDefault(u => 
                    u.Username == username && 
                    u.Password == password);

                if (user != null)
                {
                    var dashboard = new DashboardWindow(user)
                    {
                        _currentUser = user
                    };
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow()
            {
                _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer("Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;")
                    .Options)
            };
            registerWindow.Show();
            this.Close();
        }
    }
} 