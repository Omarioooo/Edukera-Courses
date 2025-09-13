using MVC_Demo.Models;

namespace MVC_Demo.ModelView
{
    public class InstructorAddModelView
    {
        public Instructore instructor { get; set; }
        public List<Department> departments { get; set; }
        public List<Course> courses { get; set; }
    }
}
