using System;
using System.Windows;
using System.IO;
using Microsoft.EntityFrameworkCore;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class DashboardWindow : Window
    {
        public required User _currentUser;
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

        public DashboardWindow(User user)
        {
            try
            {
                InitializeComponent();
                _currentUser = user;

                // Set window title with user info
                Title = $"Dashboard - {user.Username} ({user.Role})";

                // Show/hide buttons based on user role
                if (user.Role.ToLower() == "clerk")
                {
                    // Hide management buttons for clerks
                    btnStudents.Visibility = Visibility.Collapsed;
                    btnCourses.Visibility = Visibility.Collapsed;
                    btnUsers.Visibility = Visibility.Collapsed;
                }
                else if (user.Role.ToLower() == "admin")
                {
                    // Show Students and Courses buttons for admin, but hide Users
                    btnStudents.Visibility = Visibility.Visible;
                    btnCourses.Visibility = Visibility.Visible;
                    btnUsers.Visibility = Visibility.Collapsed;
                }
                // super-admin has access to all buttons by default
            }
            catch (Exception ex)
            {
                LogError(nameof(DashboardWindow), ex);
            }
        }

        private void btnStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Additional role check for security
                if (_currentUser.Role.ToLower() == "clerk")
                {
                    MessageBox.Show("Access denied. You don't have permission to manage students.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var studentManagementWindow = new StudentManagementWindow(this);
                this.Hide();
                studentManagementWindow.Show();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnStudents_Click), ex);
                this.Show(); // Make sure the dashboard is visible if an error occurs
            }
        }

        private void btnExams_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Navigate to Exams page
                MessageBox.Show("Exams section - Coming soon!");
            }
            catch (Exception ex)
            {
                LogError(nameof(btnExams_Click), ex);
            }
        }

        private void btnCourses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Additional role check for security
                if (_currentUser.Role.ToLower() == "clerk")
                {
                    MessageBox.Show("Access denied. You don't have permission to manage courses.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var courseManagementWindow = new CourseManagementWindow(this);
                this.Hide();
                courseManagementWindow.Show();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnCourses_Click), ex);
                this.Show(); // Make sure the dashboard is visible if an error occurs
            }
        }

        private void btnSeatingPlan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var coursesWindow = new Views.CoursesWindow(this);
                this.Hide();
                coursesWindow.Show();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnSeatingPlan_Click), ex);
                this.Show(); // Make sure the dashboard is visible if an error occurs
            }
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Additional role check for security
                if (_currentUser.Role.ToLower() != "super-admin")
                {
                    MessageBox.Show("Access denied. Only super-admin can manage users.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var userManagementWindow = new UserManagementWindow(_currentUser, this)
                {
                    _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseSqlServer("Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;")
                        .Options),
                    _currentUser = _currentUser,
                    _previousWindow = this
                };
                this.Hide();
                userManagementWindow.Show();
            }
            catch (Exception ex)
            {
                LogError(nameof(btnUsers_Click), ex);
                this.Show(); // Make sure the dashboard is visible if an error occurs
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LogError(nameof(btnLogout_Click), ex);
            }
        }
    }
} 