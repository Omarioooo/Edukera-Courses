using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Demo.Models
{
    public class CourseResult
    {
        public int Id { get; set; }

        public int Degree { get; set; }

        [ForeignKey("Course")]
        public int CrsID { get; set; }

        [ForeignKey("Trainees")]
        public int TraineeID { get; set; }

        public Trainee? Trainee { get; set; }

        public Course? Course { get; set; }
    }
}