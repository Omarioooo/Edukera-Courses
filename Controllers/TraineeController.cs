namespace MVC_Demo.Controllers
{
    public class TraineeController : Controller
    {
        private readonly IInstructorRepository _instRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IDepartmentRepository _deptRepo;
        private readonly ITraineeRepository _traineeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public TraineeController(IInstructorRepository instRepo, ICourseRepository courseRepo,
            IDepartmentRepository deptRepo, ITraineeRepository traineeRepo, IUnitOfWork unitOfWork)
        {
            _instRepo = instRepo;
            _courseRepo = courseRepo;
            _deptRepo = deptRepo;
            _traineeRepo = traineeRepo;
            _unitOfWork = unitOfWork;
        }

        public IActionResult ShowAll(string? search)
        {
            List<Trainee> trainees;

            if (string.IsNullOrEmpty(search))
                trainees = _traineeRepo.GetAllWithDepartment();
            else
                trainees = _traineeRepo.SearchByName(search);

            var model = trainees
            .Select(tr => new TraineeDetailsViewModel
            {
                Id = tr.Id,
                Name = tr.Name,
                Address = tr.Address,
                Grade = tr.Grade,
                ImageURL = tr.ImageURL,
                DepartmentName = tr.Department.Name
            })
            .ToList();

            return View(model);
        }

        public IActionResult Add()
        {
            var model = new TraineeFormViewModel();
            model.Departments = _deptRepo.GetAll();
            return View(model);
        }


        [HttpPost]
        public IActionResult AddSave(TraineeFormViewModel traineeRequest)
        {
            if (ModelState.IsValid)
            {
                // Add new trainee
                var newTrainee = new Trainee
                {
                    Name = traineeRequest.Name,
                    Address = traineeRequest.Address,
                    DeptID = traineeRequest.DeptID,
                    Grade = traineeRequest.Grade,
                    ImageURL = traineeRequest.ImageURL
                };

                _traineeRepo.Add(newTrainee);
                _unitOfWork.Save();

                return RedirectToAction("ShowAll");
            }

            traineeRequest.Departments = _deptRepo.GetAll();
            return View("Add", traineeRequest);
        }

        public IActionResult Details(int Id)
        {
            var trainee = _traineeRepo.GetByIdWithDepartment(Id);

            var model = new TraineeDetailsViewModel
            {
                Id = trainee.Id,
                Name = trainee.Name,
                Address = trainee.Address,
                Grade = trainee.Grade,
                DepartmentName = trainee.Department.Name
            };

            return View(model);
        }

        public IActionResult Edit(int Id)
        {
            var traineeDB = _traineeRepo.GetById(Id);

            if (traineeDB == null)
                return NotFound();

            var model = new TraineeFormViewModel()
            {
                Id = traineeDB.Id,
                Name = traineeDB.Name,
                Address = traineeDB.Address,
                Grade = traineeDB.Grade,
                DeptID = traineeDB.DeptID,
                Departments = _deptRepo.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, TraineeFormViewModel TraineeRequest)
        {
            var traineeDB = _traineeRepo.GetById(Id);

            if (traineeDB == null)
                return NotFound();


            if (ModelState.IsValid)
            {
                traineeDB.Id = TraineeRequest.Id;
                traineeDB.Name = TraineeRequest.Name;
                traineeDB.Address = TraineeRequest.Address;
                traineeDB.DeptID = TraineeRequest.DeptID;
                traineeDB.Grade = TraineeRequest.Grade;

                _unitOfWork.Save();

                return RedirectToAction("Details", new { Id });
            }

            TraineeRequest.Departments = _deptRepo.GetAll();
            return View("Edit", TraineeRequest);
        }

        public IActionResult Delete(int Id)
        {
            _traineeRepo.Delete(Id);
            _unitOfWork.Save();

            return RedirectToAction("ShowAll");
        }
    }
}
