using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using System.Linq;
using WebApplication3.Data;

namespace WebApplication3.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolDbContext _context;

        public StudentsController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? classId)
        {
            var classes = _context.Classes.ToList();
            ViewBag.Classes = classes;
            ViewBag.classId = classId;

            var students = _context.Students
                .Include(s => s.Class)
                .ThenInclude(c => c.Teacher)
                .Where(s => classId == null || s.ClassId == classId)
                .ToList();

            return View(students);
        }

        public IActionResult Create()
        {
            var classes = _context.Classes.ToList();
            if (classes == null || !classes.Any())
            {
                System.Diagnostics.Debug.WriteLine("No classes found in the database!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Classes found:");
                foreach (var cls in classes)
                {
                    System.Diagnostics.Debug.WriteLine($"Class: Id={cls.Id}, Name={cls.Name}");
                }
            }
            ViewBag.Classes = classes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Students/Create")]
        public IActionResult Create(Student student)
        {
            // Логируем входящие данные
            System.Diagnostics.Debug.WriteLine($"Received student: LastName={student.LastName}, FirstName={student.FirstName}, MiddleName={student.MiddleName}, BirthDate={student.BirthDate}, ClassId={student.ClassId}");
            var existingStudent = _context.Students
        .FirstOrDefault(s => s.LastName == student.LastName &&
                             s.FirstName == student.FirstName &&
                             s.MiddleName == student.MiddleName &&
                             s.BirthDate == student.BirthDate);
            if (existingStudent != null)
            {
                ModelState.AddModelError("", "Ученик с таким ФИО и датой рождения уже существует.");
            }



            if (!ModelState.IsValid)
            {
                // Логируем ошибки валидации
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error}");
                }

                var classes = _context.Classes.ToList();
                ViewBag.Classes = classes;
                return View(student);
            }

            _context.Add(student);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var student = _context.Students
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Students/Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}