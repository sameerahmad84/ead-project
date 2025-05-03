using System;
using System.Linq;
using System.Windows;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class AddEditCourseWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Course? _courseToEdit;
        private readonly bool _isEditMode;
        private readonly Window _previousWindow;

        public AddEditCourseWindow(Course? courseToEdit, ApplicationDbContext context, Window previousWindow)
        {
            InitializeComponent();
            _context = context;
            _courseToEdit = courseToEdit;
            _isEditMode = courseToEdit != null;
            _previousWindow = previousWindow;

            if (_isEditMode)
            {
                txtHeader.Text = "Edit Course";
                LoadCourseData();
            }
        }

        private void LoadCourseData()
        {
            if (_courseToEdit != null)
            {
                txtCourseCode.Text = _courseToEdit.CourseCode;
                txtCourseName.Text = _courseToEdit.CourseName;
                txtDescription.Text = _courseToEdit.Description;
                txtCredits.Text = _courseToEdit.Credits.ToString();
                txtDepartment.Text = _courseToEdit.Department;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtCourseCode.Text))
                {
                    MessageBox.Show("Please enter course code.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCourseName.Text))
                {
                    MessageBox.Show("Please enter course name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Please enter description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtCredits.Text, out int credits) || credits <= 0)
                {
                    MessageBox.Show("Please enter a valid number of credits.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDepartment.Text))
                {
                    MessageBox.Show("Please enter department.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if course code exists (for new courses or when course code is changed)
                var currentCourseCode = txtCourseCode.Text;
                if ((!_isEditMode || (_courseToEdit != null && _courseToEdit.CourseCode != currentCourseCode)) &&
                    _context.Courses.Any(c => c.CourseCode == currentCourseCode))
                {
                    MessageBox.Show("Course code already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_isEditMode && _courseToEdit != null)
                {
                    // Update existing course
                    _courseToEdit.CourseCode = txtCourseCode.Text;
                    _courseToEdit.CourseName = txtCourseName.Text;
                    _courseToEdit.Description = txtDescription.Text;
                    _courseToEdit.Credits = credits;
                    _courseToEdit.Department = txtDepartment.Text;
                    _courseToEdit.ModifiedDate = DateTime.Now;
                    _context.Courses.Update(_courseToEdit);
                }
                else
                {
                    // Create new course
                    var newCourse = new Course
                    {
                        CourseCode = txtCourseCode.Text,
                        CourseName = txtCourseName.Text,
                        Description = txtDescription.Text,
                        Credits = credits,
                        Department = txtDepartment.Text,
                        CreatedDate = DateTime.Now
                    };
                    _context.Courses.Add(newCourse);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving course: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 