using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Demo.Models;
using MVC_Demo.ModelView;

namespace MVC_Demo.Controllers
{
    public class CourseController : Controller
    {

        Context DbContext = new Context();
        private object course;

        public IActionResult ShowAll()
        {
            List<CourseDetailsModelView> courses = DbContext.Courses
                .Include(crs => crs.Department)
                .Select(crs => new CourseDetailsModelView
                {
                    Id = crs.Id,
                    Name = crs.Name,
                    Degree = crs.Degree,
                    MinDegree = crs.MinDegree,
                    DepartmentName = crs.Department.Name
                })
                .ToList();


            return View(courses);
        }

        public IActionResult Add()
        {
            var model = new CourseAddModelView
            {
                Course = new Course(),
                Departments = DbContext.Departments.ToList()
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult AddSave(CourseAddModelView courseRequest)
        {
            if (courseRequest.Course.Name != null)
            {
                DbContext.Courses.Add(courseRequest.Course);
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }
            return View("Add", courseRequest);
        }

        public IActionResult Details(int id)
        {
            var model = DbContext.Courses
                .Include(crs => crs.Instructores)
                .Select(crs => new CourseDetailsModelView()
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

            var model = new CourseAddModelView
            {
                Course = course,
                Departments = DbContext.Departments.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditSave(int Id, CourseAddModelView courseRequest)
        {
            var courseDB = DbContext.Courses.FirstOrDefault(crs => crs.Id == Id);
            if (courseDB == null)
            {
                return NotFound();
            }

            if (courseRequest.Course.Name != null)
            {
                courseDB.Name = courseRequest.Course.Name;
                courseDB.Degree = courseRequest.Course.Degree;
                courseDB.MinDegree = courseRequest.Course.MinDegree;
                courseDB.DeptID = courseRequest.Course.DeptID;
                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }

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
