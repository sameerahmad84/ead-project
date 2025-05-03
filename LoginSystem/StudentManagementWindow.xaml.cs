using System;
using System.Linq;
using System.Windows;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class StudentManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Window _previousWindow;

        public StudentManagementWindow(Window previousWindow)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _previousWindow = previousWindow;
            LoadStudents();
        }

        private void LoadStudents()
        {
            dgStudents.ItemsSource = _context.Students.ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditStudentWindow(null, _context, this);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadStudents();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = dgStudents.SelectedItem as Student;
            if (selectedStudent == null)
            {
                MessageBox.Show("Please select a student to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addEditWindow = new AddEditStudentWindow(selectedStudent, _context, this);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadStudents();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedStudent = dgStudents.SelectedItem as Student;
            if (selectedStudent == null)
            {
                MessageBox.Show("Please select a student to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete {selectedStudent.FirstName} {selectedStudent.LastName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Students.Remove(selectedStudent);
                    _context.SaveChanges();
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow.Show();
            this.Close();
        }

        private void btnRegisterCourses_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                var registrationWindow = new CourseRegistrationWindow(selectedStudent.Id);
                registrationWindow.Owner = this;
                registrationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a student first.", "No Student Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
} 