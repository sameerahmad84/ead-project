using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LoginSystem.Data;
using LoginSystem.Models;

namespace LoginSystem
{
    public partial class CourseRegistrationWindow : Window
    {
        private readonly int _studentId;
        private readonly List<CourseViewModel> _courses;

        public CourseRegistrationWindow(int studentId)
        {
            InitializeComponent();
            _studentId = studentId;
            _courses = new List<CourseViewModel>();
            LoadStudentInfo();
            LoadCourses();
        }

        private void LoadStudentInfo()
        {
            using (var context = new ApplicationDbContext())
            {
                var student = context.Students.Find(_studentId);
                if (student != null)
                {
                    txtStudentName.Text = $"{student.FirstName} {student.LastName}";
                }
            }
        }

        private void LoadCourses()
        {
            using (var context = new ApplicationDbContext())
            {
                var registeredCourseIds = context.StudentCourses
                    .Where(sc => sc.StudentId == _studentId)
                    .Select(sc => sc.CourseId)
                    .ToList();

                var courses = context.Courses.ToList();
                _courses.Clear();

                foreach (var course in courses)
                {
                    _courses.Add(new CourseViewModel
                    {
                        CourseId = course.Id,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Department = course.Department,
                        Credits = course.Credits,
                        IsSelected = registeredCourseIds.Contains(course.Id)
                    });
                }

                dgCourses.ItemsSource = _courses;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // Remove existing registrations
                    var existingRegistrations = context.StudentCourses
                        .Where(sc => sc.StudentId == _studentId)
                        .ToList();
                    context.StudentCourses.RemoveRange(existingRegistrations);

                    // Add new registrations
                    var selectedCourses = _courses.Where(c => c.IsSelected).ToList();
                    foreach (var course in selectedCourses)
                    {
                        context.StudentCourses.Add(new StudentCourse
                        {
                            StudentId = _studentId,
                            CourseId = course.CourseId
                        });
                    }

                    context.SaveChanges();
                    MessageBox.Show("Course registration saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving course registration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 