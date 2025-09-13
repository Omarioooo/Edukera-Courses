using MVC_Demo.Models;

namespace MVC_Demo.ModelView
{
    public class CourseAddModelView
    {
        public Course Course { get; set; }

        public List<Department> Departments { get; set; }
    }
}