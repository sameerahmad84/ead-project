using System;
using System.Linq;
using System.Windows;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class AddEditStudentWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Student? _studentToEdit;
        private readonly bool _isEditMode;
        private readonly Window _previousWindow;

        public AddEditStudentWindow(Student? studentToEdit, ApplicationDbContext context, Window previousWindow)
        {
            InitializeComponent();
            _context = context;
            _studentToEdit = studentToEdit;
            _isEditMode = studentToEdit != null;
            _previousWindow = previousWindow;

            if (_isEditMode)
            {
                txtHeader.Text = "Edit Student";
                LoadStudentData();
            }
        }

        private void LoadStudentData()
        {
            if (_studentToEdit != null)
            {
                txtFirstName.Text = _studentToEdit.FirstName;
                txtLastName.Text = _studentToEdit.LastName;
                txtEmail.Text = _studentToEdit.Email;
                txtPhoneNumber.Text = _studentToEdit.PhoneNumber;
                dpDateOfBirth.SelectedDate = _studentToEdit.DateOfBirth;
                txtDepartment.Text = _studentToEdit.Department;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    MessageBox.Show("Please enter first name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Please enter last name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please enter email.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
                {
                    MessageBox.Show("Please enter phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dpDateOfBirth.SelectedDate == null)
                {
                    MessageBox.Show("Please select date of birth.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDepartment.Text))
                {
                    MessageBox.Show("Please enter department.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if email exists (for new students or when email is changed)
                var currentEmail = txtEmail.Text;
                if ((!_isEditMode || (_studentToEdit != null && _studentToEdit.Email != currentEmail)) &&
                    _context.Students.Any(s => s.Email == currentEmail))
                {
                    MessageBox.Show("Email already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_isEditMode && _studentToEdit != null)
                {
                    // Update existing student
                    _studentToEdit.FirstName = txtFirstName.Text;
                    _studentToEdit.LastName = txtLastName.Text;
                    _studentToEdit.Email = txtEmail.Text;
                    _studentToEdit.PhoneNumber = txtPhoneNumber.Text;
                    _studentToEdit.DateOfBirth = dpDateOfBirth.SelectedDate.Value;
                    _studentToEdit.Department = txtDepartment.Text;
                    _studentToEdit.ModifiedDate = DateTime.Now;
                    _context.Students.Update(_studentToEdit);
                }
                else
                {
                    // Create new student
                    var newStudent = new Student
                    {
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        Email = txtEmail.Text,
                        PhoneNumber = txtPhoneNumber.Text,
                        DateOfBirth = dpDateOfBirth.SelectedDate.Value,
                        Department = txtDepartment.Text,
                        CreatedDate = DateTime.Now
                    };
                    _context.Students.Add(newStudent);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 