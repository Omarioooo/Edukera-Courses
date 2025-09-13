namespace MVC_Demo.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? ManagerName { get; set; }

        public List<Trainee> Trainees { get; set; }

        public List<Course> Courses { get; set; }

        public List<Instructore> Instructores { get; set; }
    }
}
