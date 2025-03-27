using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly SchoolDbContext _context;

        public ReportsController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var youngestFirstGrader = _context.Students
                .Where(s => s.Class.Name.Contains("1"))
                .OrderByDescending(s => s.BirthDate)
                .FirstOrDefault();

            var secondGradeCount = _context.Students
                .Count(s => s.Class.Name.Contains("2"));

            var teacherStats = _context.Teachers
                .Select(t => new TeacherStats
                {
                    TeacherName = t.FullName,
                    StudentCount = t.Classes.Sum(c => c.Students.Count)
                })
                .ToList();

            var model = new ReportsViewModel
            {
                YoungestFirstGrader = youngestFirstGrader,
                SecondGradeCount = secondGradeCount,
                TeacherStatistics = teacherStats
            };

            return View(model);
        }
    }

    public class ReportsViewModel
    {
        public Student YoungestFirstGrader { get; set; }
        public int SecondGradeCount { get; set; }
        public List<TeacherStats> TeacherStatistics { get; set; }
    }

    public class TeacherStats
    {
        public string TeacherName { get; set; }
        public int StudentCount { get; set; }
    }
}