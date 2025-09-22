using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace MVC_Demo.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        private readonly IInstructorRepository _instRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IDepartmentRepository _deptRepo;
        private readonly IUnitOfWork _unitOfWork;

        public InstructorController(IInstructorRepository instructorRepository, ICourseRepository courseRepository,
                           IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _instRepo = instructorRepository;
            _courseRepo = courseRepository;
            _deptRepo = departmentRepository;
            _unitOfWork = unitOfWork;
        }
        public IActionResult ShowAll(string? search)
        {
            List<Instructor> instructors;

            if (search == null)
                instructors = _instRepo.GetAllWithDepartment();
            else
                instructors = _instRepo.SearchByName(search);

            var model = instructors.Select(inst =>
            new InstructorDetailsViewModel
            {
                Id = inst.Id,
                Name = inst.Name,
                ImageURL = inst.ImageURL,
                DepartmentName = inst.Department.Name

            })
            .ToList();

            return View(model);
        }

        public IActionResult Add()
        {
            var instructorModel = new InstructorFormViewModel
            {
                Departments = _deptRepo.GetAll(),
                Courses = _courseRepo.GetAll()
            };

            return View(instructorModel);
        }

        [HttpPost]
        public IActionResult AddSave(InstructorFormViewModel instructorRequest)
        {
            if (ModelState.IsValid)
            {
                var instructor = new Instructor()
                {
                    Name = instructorRequest.Name,
                    Salary = instructorRequest.Salary,
                    Address = instructorRequest.Address,
                    ImageURL = instructorRequest.ImageURL,
                    CrsID = instructorRequest.CrsID,
                    DeptID = instructorRequest.DeptID
                };

                _instRepo.Add(instructor);

                _unitOfWork.Save();

                return RedirectToAction("ShowAll");
            }

            instructorRequest.Departments = _deptRepo.GetAll();
            instructorRequest.Courses = _courseRepo.GetAll();
            return View("Add", instructorRequest);
        }

        public IActionResult Details(int Id)
        {
            var instructor = _instRepo.GetByIDWithDepartment(Id);

            if (instructor == null)
                return NotFound();
            return View(instructor);
        }

        public IActionResult Edit(int Id)
        {
            var instructor = _instRepo.GetById(Id);

            var departments = _deptRepo.GetAll();
            var courses = _courseRepo.GetAll();

            var instructorModel = new InstructorFormViewModel()
            {
                Id = instructor.Id,
                Name = instructor.Name,
                Salary = instructor.Salary,
                Address = instructor.Address,
                ImageURL = instructor.ImageURL,
                CrsID = instructor.CrsID,
                DeptID = instructor.DeptID,
                Departments = departments,
                Courses = courses
            };

            return View(instructorModel);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, InstructorFormViewModel instructorRequest)
        {
            var instructor = _instRepo.GetById(Id);

            if (instructor == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                instructor.Name = instructorRequest.Name;
                instructor.Address = instructorRequest.Address;
                instructor.ImageURL = instructorRequest.ImageURL;
                instructor.Salary = instructorRequest.Salary;
                instructor.DeptID = instructorRequest.DeptID;
                instructor.CrsID = instructorRequest.CrsID;

                _unitOfWork.Save();

                return RedirectToAction("Details", new { Id });
            }

            instructorRequest.Departments = _deptRepo.GetAll();
            instructorRequest.Courses = _courseRepo.GetAll();
            return View("Edit", instructorRequest);
        }

        public IActionResult Delete(int Id)
        {
            _instRepo.Delete(Id);

            // Save all changes
            _unitOfWork.Save();

            return RedirectToAction("ShowAll");
        }
    }
}