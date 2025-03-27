using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Linq;
using System;

namespace WebApplication3.Controllers
{
    public class ReportsController : Controller
    {
        private readonly SchoolDbContext _context;

        public ReportsController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ReportViewModel();

            // Текущая дата
            var currentDate = DateTime.Now;

            // 1. Самый старший первоклассник (с самой ранней датой рождения)
            var oldestFirstGraderQuery = (from s in _context.Students
                                          join c in _context.Classes on s.ClassId equals c.Id
                                          where c.Name.StartsWith("1")
                                          orderby s.BirthDate ascending // Сортируем по возрастанию, чтобы найти самую раннюю дату
                                          select new ReportViewModel.YoungestFirstGrader
                                          {
                                              LastName = s.LastName ?? "Не указано",
                                              FirstName = s.FirstName ?? "Не указано",
                                              MiddleName = s.MiddleName ?? "Не указано",
                                              BirthDate = s.BirthDate,
                                              ClassName = c.Name ?? "Не указано"
                                          });
            model.YoungestFirstGraderData = oldestFirstGraderQuery.FirstOrDefault();

            // 2. Количество учеников во всех вторых классах
            model.SecondGradeStudentsCount = (from s in _context.Students
                                              join c in _context.Classes on s.ClassId equals c.Id
                                              where c.Name.StartsWith("2")
                                              select s).Count();

            // 3. Количество учеников у каждого классного руководителя
            var classesWithTeachers = (from c in _context.Classes
                                       join t in _context.Teachers on c.TeacherId equals t.Id into teacherGroup
                                       from t in teacherGroup.DefaultIfEmpty()
                                       select new
                                       {
                                           ClassId = c.Id,
                                           TeacherName = t != null ? t.FullName : "Не назначен"
                                       }).ToList();

            model.StudentsPerTeacherData = (from ct in classesWithTeachers
                                            join s in _context.Students on ct.ClassId equals s.ClassId into studentGroup
                                            from s in studentGroup.DefaultIfEmpty()
                                            group s by new { ct.TeacherName } into g
                                            select new ReportViewModel.StudentsPerTeacher
                                            {
                                                TeacherName = g.Key.TeacherName,
                                                StudentCount = g.Count(s => s != null)
                                            }).ToList();

            return View(model);
        }
    }
}