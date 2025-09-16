using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVC_Demo.Models;
using MVC_Demo.Repository;
using MVC_Demo.ViewModels;

namespace MVC_Demo.Controllers
{
    public class CourseController : Controller
    {

        Context DbContext = new Context();

        public IActionResult ShowAll(string? search)
        {
            var coursesQuery = DbContext.Courses
                .Include(crs => crs.Department)
                .Select(crs => new CourseDetailsViewModel
                {
                    Id = crs.Id,
                    Name = crs.Name,
                    Degree = crs.Degree,
                    MinDegree = crs.MinDegree,
                    DepartmentName = crs.Department.Name
                });

            if (!search.IsNullOrEmpty())
            {
                var coursesList = coursesQuery
                     .Where(crs => crs.Name.StartsWith(search))
                     .ToList();

                return View(coursesList);
            }

            var courses = coursesQuery.ToList();
            return View(courses);
        }

        public IActionResult Add()
        {
            var model = new CourseFormViewModel
            {
                Departments = DbContext.Departments.ToList()
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
                DbContext.Courses.Add(course);

                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }

            courseRequest.Departments = DbContext.Departments.ToList();
            return View("Add", courseRequest);
        }

        public IActionResult Details(int id)
        {
            var model = DbContext.Courses
                .Include(crs => crs.Instructores)
                .Select(crs => new CourseDetailsViewModel()
                {
                    Id = crs.Id,
                    Name = crs.Name,
                    Degree = crs.Degree,
                    MinDegree = crs.MinDegree,
                    NumOfInstructors = crs.Instructores.Count(),
                    DepartmentName = crs.Department.Name
                })
                .FirstOrDefault(crs => crs.Id == id);

            return View(model);
        }

        public IActionResult Edit(int Id)
        {
            var course = DbContext.Courses.FirstOrDefault(crs => crs.Id == Id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseFormViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Degree = course.Degree,
                MinDegree = course.MinDegree,
                DeptID = course.DeptID,
                Departments = DbContext.Departments.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, CourseFormViewModel courseRequest)
        {
            var courseDB = DbContext.Courses.FirstOrDefault(crs => crs.Id == Id);
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
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

            courseRequest.Departments = DbContext.Departments.ToList();
            return View("Edit", courseRequest);
        }

        public IActionResult Delete(int Id)
        {
            var course = DbContext.Courses
                .FirstOrDefault(c => c.Id == Id);

            if (course != null)
            {
                // Delete related instructors
                var instructors = DbContext.Instructores
                    .Where(inst => inst.Course.Id == Id)
                    .ToList();
                DbContext.Instructores.RemoveRange(instructors);

                // Delete related results
                var results = DbContext.Results
                    .Where(rs => rs.Course.Id == Id)
                    .ToList();
                DbContext.Results.RemoveRange(results);

                // Delete the course itself
                DbContext.Courses.Remove(course);

                // Save all changes
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }
            return RedirectToAction("Details", new { Id });
        }
    }
}