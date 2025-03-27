using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Linq;
using System;

namespace WebApplication3.Controllers
{
    public class ClassesController : Controller
    {
        private readonly SchoolDbContext _context;

        public ClassesController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var classes = _context.Classes
                .Include(c => c.Teacher)
                .ToList();
            return View(classes);
        }

        public IActionResult AssignTeacher(int id)
        {
            var classToAssign = _context.Classes.Find(id);
            if (classToAssign == null)
            {
                return NotFound();
            }

            var teachers = _context.Teachers.ToList();
            if (teachers == null || !teachers.Any())
            {
                System.Diagnostics.Debug.WriteLine("No teachers found in the database!");
            }
            ViewBag.Teachers = teachers;
            ViewBag.ClassName = classToAssign.Name;
            return View(classToAssign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignTeacher(int id, int teacherId)
        {
            var classToAssign = _context.Classes.Find(id);
            if (classToAssign == null)
            {
                return NotFound();
            }

            var teacher = _context.Teachers.Find(teacherId);
            if (teacher == null)
            {
                ModelState.AddModelError("TeacherId", "Выбранный преподаватель не существует.");
            }

            if (!ModelState.IsValid)
            {
                var teachers = _context.Teachers.ToList();
                ViewBag.Teachers = teachers;
                ViewBag.ClassName = classToAssign.Name;
                return View(classToAssign);
            }

            try
            {
                classToAssign.TeacherId = teacherId;
                _context.Update(classToAssign);
                _context.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"Teacher {teacherId} assigned to class {id} successfully!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error assigning teacher: {ex.Message}");
                ModelState.AddModelError("", "Ошибка при назначении классного руководителя: " + ex.Message);
                var teachers = _context.Teachers.ToList();
                ViewBag.Teachers = teachers;
                ViewBag.ClassName = classToAssign.Name;
                return View(classToAssign);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}