using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext (DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable(nameof(Student));

            modelBuilder.Entity<Enrollment>().ToTable(nameof(Enrollment));

            modelBuilder.Entity<Course>().ToTable(nameof(Course))
                .HasMany(course => course.Instructors).WithMany(instructor => instructor.Courses);

            modelBuilder.Entity<Department>()
                .ToTable(nameof(Department))
                .Property(department => department.ConcurrencyToken).IsConcurrencyToken();

            modelBuilder.Entity<Instructor>().ToTable(nameof(Instructor));

            modelBuilder.Entity<OfficeAssignment>().ToTable(nameof(OfficeAssignment));
        }
    }
}
