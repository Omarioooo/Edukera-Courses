using Microsoft.EntityFrameworkCore;

namespace MVC_Demo.Models
{
    public class Context : DbContext
    {
        public Context() : base() { }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=CoursesDB;Trusted_Connection=True;TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructore> Instructores { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseResult> Results { get; set; }
    }
}
