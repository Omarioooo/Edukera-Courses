namespace MVC_Demo.ModelView
{
    public class DepartmentDetailsModelView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }

        public int NumberOfInstructors { get; set; }

        public int NumberOfTrainees { get; set; }
        public int NumberOfCourses { get; set; }
    }
}
