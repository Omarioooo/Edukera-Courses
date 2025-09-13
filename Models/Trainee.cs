using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Demo.Models
{
    public class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? ImageURL { get; set; }

        [NotMapped]
        public IFormFile? imageFile { get; set; }
        public string? Address { get; set; }

        public string? Grade { get; set; }

        [ForeignKey("Department")]
        [Display(Name = "Department")]
        public int DeptID { get; set; }

        public Department? Department { get; set; }

        public List<CourseResult>? Results { get; set; }
    }
}
