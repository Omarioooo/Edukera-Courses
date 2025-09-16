using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVC_Demo.Models;
using MVC_Demo.Repository;
using MVC_Demo.ViewModels;

namespace MVC_Demo.Controllers
{
    public class TraineeController : Controller
    {
        Context DbContext = new Context();

        public IActionResult ShowAll(string? search)
        {

            var traineesQuery = DbContext.Trainees
                .Include(tr => tr.Department)
                .Select(tr => new TraineeDetailsViewModel
                {
                    Id = tr.Id,
                    Name = tr.Name,
                    Address = tr.Address,
                    ImageURL = tr.ImageURL,
                    Grade = tr.Grade,
                    DepartmentName = tr.Department.Name
                })
                .AsQueryable();

            if (!search.IsNullOrEmpty())
            {
                var traineesList = traineesQuery
                    .Where(tr => tr.Name.StartsWith(search))
                    .ToList();

                return View(traineesList);
            }

            var trainees = traineesQuery.ToList();
            return View(trainees);
        }

        public IActionResult Add()
        {
            var departments = DbContext.Departments.ToList();
            ViewBag.Departments = departments;
            return View();
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

                DbContext.Trainees.Add(newTrainee);
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }

            traineeRequest.Departments = DbContext.Departments.ToList();
            return View("Add", traineeRequest);
        }

        public IActionResult Details(int id)
        {
            var traineeModel = DbContext.Trainees
                .Include(tr => tr.Department)
                .Select(tr => new TraineeDetailsViewModel
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

            var Model = new TraineeFormViewModel()
            {
                Id = traineeDB.Id,
                Name = traineeDB.Name,
                Address = traineeDB.Address,
                Grade = traineeDB.Grade,
                DeptID = traineeDB.DeptID,
                Departments = departments
            };

            return View(Model);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, TraineeFormViewModel TraineeRequest)
        {
            var traineeDB = DbContext.Trainees.FirstOrDefault(tr => tr.Id == Id);
            if (traineeDB == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                traineeDB.Id = TraineeRequest.Id;
                traineeDB.Name = TraineeRequest.Name;
                traineeDB.Address = TraineeRequest.Address;
                traineeDB.DeptID = TraineeRequest.DeptID;
                traineeDB.Grade = TraineeRequest.Grade;
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

            TraineeRequest.Departments = DbContext.Departments.ToList();
            return View("Edit", TraineeRequest);
        }

        public IActionResult Delete(int Id)
        {
            var trainee = DbContext.Trainees.FirstOrDefault(tr => tr.Id == Id);

            if (trainee != null)
            {
                // Remoce Related Results
                var results = DbContext.Results
                    .Where(rs => rs.TraineeID == Id)
                    .ToList();
                DbContext.RemoveRange(results);

                // Remove Course it Self
                DbContext.Trainees.Remove(trainee);
                DbContext.SaveChanges(true);

                return RedirectToAction("ShowAll");
            }

            return RedirectToAction("Details", new { Id });
        }
    }
}
