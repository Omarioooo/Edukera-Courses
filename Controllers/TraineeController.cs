using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Demo.Models;
using MVC_Demo.ModelView;

namespace MVC_Demo.Controllers
{
    public class TraineeController : Controller
    {
        Context DbContext = new Context();

        public IActionResult ShowAll()
        {

            var traineesModel = DbContext.Trainees
                .Include(tr => tr.Department)
                .Select(tr => new TraineeShowAllModelView
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Address = tr.Address,
                    ImageURL = tr.ImageURL,
                    DepartmentName = tr.Department.Name
                })
                .ToList();
            return View(traineesModel);
        }

        public IActionResult Add(TraineeAddModelView traineeRequest)
        {
            var departments = DbContext.Departments.ToList();
            traineeRequest.Departments = departments;

            return View(traineeRequest);
        }


        [HttpPost]
        public IActionResult AddSave(TraineeAddModelView traineeRequest)
        {
            if (traineeRequest.Trainee?.Name == null)
            {
                traineeRequest.Departments = DbContext.Departments.ToList();
                return View("Add", traineeRequest);
            }

            // Add new trainee
            var newTrainee = new Trainee
            {
                Name = traineeRequest.Trainee.Name,
                Address = traineeRequest.Trainee.Address,
                DeptID = traineeRequest.Trainee.DeptID,
                Grade = traineeRequest.Trainee.Grade,
                ImageURL = traineeRequest.Trainee.ImageURL
            };

            DbContext.Trainees.Add(newTrainee);
            DbContext.SaveChanges();

            return RedirectToAction("ShowAll");
        }

        public IActionResult Details(int id)
        {
            var traineeModel = DbContext.Trainees
                .Include(tr => tr.Department)
                .Select(tr => new TraineeDetailsModelView
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Address = tr.Address,
                    Grade = tr.Grade,
                    DepartmentName = tr.Department.Name
                })
                .FirstOrDefault(tr => tr.Id == id);
            return View(traineeModel);
        }

        public IActionResult Edit(int id)
        {
            var traineeDB = DbContext.Trainees.FirstOrDefault(tr => tr.Id == id);
            if (traineeDB == null)
            {
                return NotFound();
            }
            var departments = DbContext.Departments.ToList();


            return View(new TraineeAddModelView() { Trainee = traineeDB, Departments = departments });
        }

        [HttpPost]
        public IActionResult EditSave(int Id, TraineeAddModelView modelRequest)
        {
            var traineeDB = DbContext.Trainees.FirstOrDefault(tr => tr.Id == Id);
            if (traineeDB == null)
            {
                return NotFound();
            }

            if (modelRequest.Trainee.Name != null)
            {
                traineeDB.Name = modelRequest.Trainee.Name;
                traineeDB.Address = modelRequest.Trainee.Address;
                traineeDB.DeptID = modelRequest.Trainee.DeptID;
                traineeDB.Grade = modelRequest.Trainee.Grade;
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

            return View("Edit", modelRequest);
        }
    }
}
