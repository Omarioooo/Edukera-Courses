using Microsoft.AspNetCore.Authorization;

namespace MVC_Demo.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {

        private readonly ICourseRepository _courseRepo;
        private readonly IDepartmentRepository _deptRepo;
        private readonly IInstructorRepository _instRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CourseController(ICourseRepository courseRepo, IDepartmentRepository deptRepo,
                         IInstructorRepository instRepo, IUnitOfWork unitOfWork)
        {
            _courseRepo = courseRepo;
            _deptRepo = deptRepo;
            _instRepo = instRepo;
            _unitOfWork = unitOfWork;
        }

        public IActionResult ShowAll(string? search)
        {
            List<Course> courses;

            if (string.IsNullOrEmpty(search))
                courses = _courseRepo.GetAllWithDepartment();
            else
                courses = _courseRepo.SearchByName(search);

            var model = courses.Select(crs =>
                new CourseDetailsViewModel()
                {
                    Id = crs.Id,
                    Name = crs.Name,
                    Degree = crs.Degree,
                    MinDegree = crs.MinDegree,
                    DepartmentName = crs.Department?.Name
                })
                .ToList();

            return View(model);
        }

        public IActionResult Add()
        {
            var model = new CourseFormViewModel
            {
                Departments = _deptRepo.GetAll()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult AddSave(CourseFormViewModel courseRequest)
        {
            if (ModelState.IsValid)
            {
                var course = new Course()
                {
                    Name = courseRequest.Name,
                    Degree = courseRequest.Degree,
                    MinDegree = courseRequest.MinDegree,
                    DeptID = courseRequest.DeptID,
                };
                _courseRepo.Add(course);

                _unitOfWork.Save();

                return RedirectToAction("ShowAll");
            }

            courseRequest.Departments = _deptRepo.GetAll();
            return View("Add", courseRequest);
        }

        public IActionResult Details(int Id)
        {
            var course = _courseRepo.GetByIdWithDepartment(Id);

            var model = new CourseDetailsViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Degree = course.Degree,
                MinDegree = course.MinDegree,
                DepartmentName = course.Department.Name,
                NumOfInstructors = _instRepo.NumOfInstructorsByCourse(Id)
            };

            return View(model);
        }

        public IActionResult Edit(int Id)
        {
            var course = _courseRepo.GetById(Id);
            if (course == null)
                return NotFound();

            var model = new CourseFormViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Degree = course.Degree,
                MinDegree = course.MinDegree,
                DeptID = course.DeptID,
                Departments = _deptRepo.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, CourseFormViewModel courseRequest)
        {
            var courseDB = _courseRepo.GetById(Id);
            if (courseDB == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                courseDB.Name = courseRequest.Name;
                courseDB.Degree = courseRequest.Degree;
                courseDB.MinDegree = courseRequest.MinDegree;
                courseDB.DeptID = courseRequest.DeptID;

                _unitOfWork.Save();

                return RedirectToAction("Details", new { Id });
            }

            courseRequest.Departments = _deptRepo.GetAll();
            return View("Edit", courseRequest);
        }

        public IActionResult Delete(int Id)
        {
            // Delete the course itself
            _courseRepo.Delete(Id);

            // Delete related instructors
            var instructors = _instRepo.SearchByCourseID(Id);
            _instRepo.DeleteGroup(instructors);

            // Save updates
            _unitOfWork.Save();

            return RedirectToAction("ShowAll");
        }
    }
}