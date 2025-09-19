using Microsoft.IdentityModel.Tokens;
namespace MVC_Demo.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _deptRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly ITraineeRepository _traineeRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IDepartmentRepository deptRepo, IUnitOfWork unitOfWork)
        {
            _deptRepo = deptRepo;
            _unitOfWork = unitOfWork;
        }

        public IActionResult ShowAll(string? search)
        {
            List<Department> departments;

            if (search.IsNullOrEmpty())
                departments = _deptRepo.GetAll();
            else
                departments = _deptRepo.SearchByName(search);

            return View(departments);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSave(Department departmentRequest)
        {
            if (ModelState.IsValid)
            {
                _deptRepo.Add(departmentRequest);

                _unitOfWork.Save();

                return RedirectToAction("ShowAll", _deptRepo.GetAll());
            }

            return View("Add", departmentRequest);
        }

        public IActionResult Details(int Id)
        {
            var department = _deptRepo.GetByIdWithRelations(Id);


            var model = new DepartmentDetailsViewModel()
            {
                Id = department.Id,
                Name = department.Name,
                ManagerName = department.ManagerName,
                NumberOfInstructors = _deptRepo.GetNumberOfInstructors(Id),
                NumberOfCourses = _deptRepo.GetNumberOfCourses(Id),
                NumberOfTrainees = _deptRepo.GetNumberOfTrainees(Id),
            };

            return View(model);
        }

        public IActionResult Edit(int Id)
        {
            var department = _deptRepo.GetById(Id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public IActionResult Editsave(int Id, Department departmentRequest)
        {
            if (ModelState.IsValid)
            {
                var departmentDB = _deptRepo.GetById(Id);
                departmentDB.Name = departmentRequest.Name;
                departmentDB.ManagerName = departmentRequest.ManagerName;

                // Save Changes
                _unitOfWork.Save();

                return RedirectToAction("Details", new { Id });
            }
            return View("Edit", departmentRequest);
        }

        public IActionResult Delete(int Id)
        {
            // Remove Related Instructor
            var instructors = _instructorRepo.GetByDepartment(Id);
            _instructorRepo.DeleteGroup(instructors);

            // Remove Related Trainees
            var trainees = _traineeRepo.GetByDepartment(Id);
            _traineeRepo.DeleteGroup(trainees);

            // Remove Realated Coureses
            var courses = _courseRepo.GetByDepartment(Id);
            _courseRepo.DeleteGroup(courses);

            // Remove Course itself
            _deptRepo.Delete(Id);

            // Save all changes
            _unitOfWork.Save();

            return RedirectToAction("ShowAll");
        }
    }
}
