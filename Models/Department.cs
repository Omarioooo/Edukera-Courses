using MVC_Demo.Validation;
using System.ComponentModel.DataAnnotations;

namespace MVC_Demo.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [UniqueDepartment]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Only letters are allowed.")]
        public string? ManagerName { get; set; }

        public List<Trainee>? Trainees { get; set; }

        public List<Course>? Courses { get; set; }

        public List<Instructore>? Instructores { get; set; }
    }
}
