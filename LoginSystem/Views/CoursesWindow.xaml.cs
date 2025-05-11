using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using LoginSystem.Data;
using LoginSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Views
{
    public partial class CoursesWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Window _previousWindow;
        private Course _selectedCourse;
        private List<Student> _selectedCourseStudents;

        public CoursesWindow(Window previousWindow = null)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _previousWindow = previousWindow;
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                var courses = _context.Courses
                    .Include(c => c.StudentCourses)
                        .ThenInclude(sc => sc.Student)
                    .ToList();

                CourseComboBox.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading courses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CourseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CourseComboBox.SelectedItem is Course selectedCourse)
            {
                _selectedCourse = selectedCourse;
                _selectedCourseStudents = selectedCourse.StudentCourses
                    .Select(sc => sc.Student)
                    .ToList();

                StudentsListView.ItemsSource = _selectedCourseStudents;
            }
        }

        private void GenerateSeatingPlanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedCourse == null)
                {
                    MessageBox.Show("Please select a course first.", "No Course Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(RowsTextBox.Text, out int rows) || !int.TryParse(ColumnsTextBox.Text, out int columns))
                {
                    MessageBox.Show("Please enter valid numbers for rows and columns.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (rows <= 0 || columns <= 0)
                {
                    MessageBox.Show("Rows and columns must be greater than 0.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var totalSeats = rows * columns;
                if (totalSeats < _selectedCourseStudents.Count)
                {
                    MessageBox.Show($"Not enough seats for all students. Need {_selectedCourseStudents.Count} seats but only have {totalSeats}.", 
                        "Insufficient Seats", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create seating plan
                var seatingPlan = new List<List<SeatInfo>>();
                var random = new Random();
                var shuffledStudents = _selectedCourseStudents.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < rows; i++)
                {
                    var row = new List<SeatInfo>();
                    for (int j = 0; j < columns; j++)
                    {
                        int index = i * columns + j;
                        var seatInfo = new SeatInfo
                        {
                            StudentName = index < shuffledStudents.Count 
                                ? $"{shuffledStudents[index].FirstName} {shuffledStudents[index].LastName}"
                                : "Empty"
                        };
                        row.Add(seatInfo);
                    }
                    seatingPlan.Add(row);
                }

                SeatingPlanGrid.ItemsSource = seatingPlan;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating seating plan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context.Dispose();
            _previousWindow?.Show();
        }
    }

    public class SeatInfo
    {
        public string StudentName { get; set; }
    }
} 