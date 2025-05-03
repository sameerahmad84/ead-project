using System;
using System.Linq;
using System.Windows;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class CourseManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Window _previousWindow;

        public CourseManagementWindow(Window previousWindow)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _previousWindow = previousWindow;
            LoadCourses();
        }

        private void LoadCourses()
        {
            dgCourses.ItemsSource = _context.Courses.ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditCourseWindow(null, _context, this);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadCourses();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedCourse = dgCourses.SelectedItem as Course;
            if (selectedCourse == null)
            {
                MessageBox.Show("Please select a course to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addEditWindow = new AddEditCourseWindow(selectedCourse, _context, this);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadCourses();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCourse = dgCourses.SelectedItem as Course;
            if (selectedCourse == null)
            {
                MessageBox.Show("Please select a course to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete {selectedCourse.CourseName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Courses.Remove(selectedCourse);
                    _context.SaveChanges();
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow.Show();
            this.Close();
        }
    }
} 