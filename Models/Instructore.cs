using MVC_Demo.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Demo.Models
{
    public class Instructore
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? ImageURL { get; set; }

        public Decimal? Salary { get; set; }

        public string? Address { get; set; }

        [ForeignKey("Department")]
        public int DeptID { get; set; }

        [ForeignKey("Course")]
        public int CrsID { get; set; }

        public Department? Department { get; set; }

        public Course? Course { get; set; }
    }
}
