using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Demo.Models;
using MVC_Demo.ModelView;

namespace MVC_Demo.Controllers
{
    public class DepartmentController : Controller
    {
        Context DbContext = new Context();

        public IActionResult ShowAll()
        {
            var departments = DbContext.Departments.Select(dept => dept).ToList();
            return View(departments);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSave(Department departmentRequest)
        {
            if (departmentRequest.Name != null && departmentRequest.ManagerName != null)
            {
                DbContext.Departments.Add(departmentRequest);
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll", DbContext.Departments.ToList());
            }

            return View("Add", departmentRequest);
        }

        public IActionResult Details(int Id)
        {
            var department = DbContext.Departments
              .Include(dept => dept.Courses)
              .Include(dept => dept.Trainees)
              .Include(dept => dept.Instructores)
              .FirstOrDefault(dept => dept.Id == Id);


            var departmentModel = new DepartmentDetailsModelView()
            {
                Id = department.Id,
                Name = department.Name,
                ManagerName = department.ManagerName,
                NumberOfInstructors = department.Instructores.Count(),
                NumberOfCourses = department.Courses.Count(),
                NumberOfTrainees = department.Trainees.Count(),
            };

            return View(departmentModel);
        }

        public IActionResult Edit(int id)
        {
            var department = DbContext.Departments.FirstOrDefault(dept => dept.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public IActionResult Editsave(int Id, Department departmentRequest)
        {
            if (departmentRequest.Name != null && departmentRequest.ManagerName != null)
            {
                var departmentDB = DbContext.Departments.FirstOrDefault(dept => dept.Id == Id);
                departmentDB.Name = departmentRequest.Name;
                departmentDB.ManagerName = departmentRequest.ManagerName;

                DbContext.SaveChanges();

                return RedirectToAction("Details", new { Id });
            }
            return View("Edit", departmentRequest);
        }

        public IActionResult Delete(int Id)
        {
            var department = DbContext.Departments.FirstOrDefault(dept => dept.Id == Id);

            if (department != null)
            {
                // Remove Related Instructor
                var instructors = DbContext.Instructores
                    .Where(inst => inst.DeptID == Id)
                    .ToList();
                DbContext.RemoveRange(instructors);

                // Remove Related Trainees
                var trainees = DbContext.Trainees
                    .Where(tr => tr.DeptID == Id)
                    .ToList();
                DbContext.RemoveRange(trainees);

                // Remove Realated Coureses
                var courses = DbContext.Courses
                    .Where(crs => crs.DeptID == Id)
                    .ToList();
                DbContext.RemoveRange(courses);

                // Remove Course itself
                DbContext.Remove(department);

                // Save all changes
                DbContext.SaveChanges();

                return RedirectToAction("ShowAll");
            }

            return RedirectToAction("Details", new { Id });
        }
    }
}
