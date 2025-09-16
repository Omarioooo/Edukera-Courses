using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVC_Demo.Models;
using MVC_Demo.Repository;
using MVC_Demo.ViewModels;

namespace MVC_Demo.Controllers
{
    public class InstructorController : Controller
    {
        Context DbContext = new Context();

        public IActionResult ShowAll(string? search)
        {
            var instructorsQuery = DbContext.Instructores
                .Include(inst => inst.Department)
                .Select(inst => new InstructorDetailsViewModel
                {
                    Id = inst.Id,
                    Name = inst.Name,
                    ImageURL = inst.ImageURL,
                    DepartmentName = inst.Department.Name
                })
                .AsQueryable();

            if (!search.IsNullOrEmpty())
            {
                var instructorsList = instructorsQuery
                    .Where(inst => inst.Name.StartsWith(search))
                    .ToList();

                return View(instructorsList);
            }

            var instructors = instructorsQuery.ToList();
            return View(instructors);
        }

        public IActionResult Add()
        {
            var instructorModel = new InstructorFormViewModel
            {
                Departments = DbContext.Departments.ToList(),
                Courses = DbContext.Courses.ToList()
            };

            return View(instructorModel);
        }

        [HttpPost]
        public IActionResult AddSave(InstructorFormViewModel instructorRequest)
        {
            if (ModelState.IsValid)
            {
                var instructor = new Instructore()
                {
                    Name = instructorRequest.Name,
                    Salary = instructorRequest.Salary,
                    Address = instructorRequest.Address,
                    ImageURL = instructorRequest.ImageURL,
                    CrsID = instructorRequest.CrsID,
                    DeptID = instructorRequest.DeptID
                };

                DbContext.Instructores.Add(instructor);
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }

            instructorRequest.Departments = DbContext.Departments.ToList();
            instructorRequest.Courses = DbContext.Courses.ToList();
            return View("Add", instructorRequest);
        }

        public IActionResult Details(int Id)
        {
            var instructore = DbContext.Instructores
                .Include(inst => inst.Department)
                .Include(inst => inst.Course)
                .FirstOrDefault(inst => inst.Id == Id);

            if (instructore == null)
            {
                return NotFound();
            }

            return View(instructore);
        }

        public IActionResult Edit(int Id)
        {
            var instructor = DbContext.Instructores
                .FirstOrDefault(inst => inst.Id == Id);

            var departments = DbContext.Departments.ToList();
            var courses = DbContext.Courses.ToList();

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
            var instructor = DbContext.Instructores
                .FirstOrDefault(inst => inst.Id == Id);

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
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

            instructorRequest.Departments = DbContext.Departments.ToList();
            instructorRequest.Courses = DbContext.Courses.ToList();
            return View("Edit", instructorRequest);
        }

        public IActionResult Delete(int Id)
        {
            var instructor = DbContext.Instructores.FirstOrDefault(inst => inst.Id == Id);

            if (instructor != null)
            {
                DbContext.Instructores.Remove(instructor);

                // Save all changes
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }
            return RedirectToAction("Details", new { Id });
        }
    }
}
