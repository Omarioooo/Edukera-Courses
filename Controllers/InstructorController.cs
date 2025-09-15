using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVC_Demo.Models;
using MVC_Demo.ModelView;

namespace MVC_Demo.Controllers
{
    public class InstructorController : Controller
    {
        Context DbContext = new Context();

        public IActionResult ShowAll(string? search)
        {
            var instructorsQuery = DbContext.Instructores
                .Include(inst => inst.Department)
                .Select(inst => new InstructorShowAllModelView
                {
                    ID = inst.Id,
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
            var instructorModel = new InstructorAddModelView
            {
                instructor = new Instructore(),
                departments = DbContext.Departments.ToList(),
                courses = DbContext.Courses.ToList()
            };

            return View(instructorModel);
        }

        [HttpPost]
        public IActionResult AddSave(InstructorAddModelView instructoreRequest)
        {
            if (instructoreRequest.instructor.Name != null)
            {
                DbContext.Instructores.Add(instructoreRequest.instructor);
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }

            return View("Add", instructoreRequest);
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

            InstructorAddModelView instructorModel = new InstructorAddModelView()
            {
                instructor = instructor,
                departments = departments,
                courses = courses
            };
            return View(instructorModel);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, InstructorAddModelView instructorRequest)
        {
            var instructor = DbContext.Instructores
                .FirstOrDefault(inst => inst.Id == Id);

            if (instructor == null)
            {
                return NotFound();
            }

            if (instructorRequest.instructor.Name != null)
            {
                instructor.Name = instructorRequest.instructor.Name;
                instructor.Address = instructorRequest.instructor.Address;
                instructor.ImageURL = instructorRequest.instructor.ImageURL;
                instructor.Salary = instructorRequest.instructor.Salary;
                instructor.DeptID = instructorRequest.instructor.DeptID;
                instructor.CrsID = instructorRequest.instructor.CrsID;
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

            instructorRequest.instructor = instructor;
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
