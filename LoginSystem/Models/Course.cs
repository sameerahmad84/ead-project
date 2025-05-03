using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LoginSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(200)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation property for students
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
} 