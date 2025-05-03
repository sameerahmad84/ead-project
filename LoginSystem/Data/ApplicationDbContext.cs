using Microsoft.EntityFrameworkCore;
using LoginSystem.Models;

namespace LoginSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Enable automatic database creation and migration
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(
                        "Server=SAMEER-AHMAD;Database=LoginSystemDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=True;Connect Timeout=30;",
                        opt => opt.EnableRetryOnFailure())
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasDefaultValue("clerk");
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Course entity
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CourseCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CourseName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.CourseCode).IsUnique();
            });

            // Configure StudentCourse entity
            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Configure the many-to-many relationship
                entity.HasOne(e => e.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Add unique constraint to prevent duplicate registrations
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            });
        }
    }
} 