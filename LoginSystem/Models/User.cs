using System;
using System.ComponentModel.DataAnnotations;

namespace LoginSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        public required string Password { get; set; }

        [StringLength(100)]
        public required string Email { get; set; }

        [Required]
        [StringLength(20)]
        public required string Role { get; set; } = "super-admin"; // Default role

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
} 