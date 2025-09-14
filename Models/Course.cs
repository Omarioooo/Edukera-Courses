using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Demo.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Degree { get; set; }

        public int MinDegree { get; set; }

        [ForeignKey("Department")]
        [Display(Name = "Department")]
        public int DeptID { get; set; }

        public Department? Department { get; set; }

        public List<Instructore>? Instructores { get; set; }
        public List<CourseResult>? Results { get; set; }
    }
}
