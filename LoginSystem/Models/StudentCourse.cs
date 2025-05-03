using System;
using System.ComponentModel.DataAnnotations;

namespace LoginSystem.Models
{
    public class StudentCourse
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime RegistrationDate { get; set; }
        public string? Grade { get; set; }
        public bool IsActive { get; set; }
    }
} 