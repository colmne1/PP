using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public ReportsController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/reports?classId={id}
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReportViewModel>> GetReports(int? classId)
        {
            try
            {
                var studentsQuery = _context.Students
                    .Include(s => s.Class)
                    .ThenInclude(c => c.Teacher)
                    .AsQueryable();

                if (classId.HasValue)
                {
                    studentsQuery = studentsQuery.Where(s => s.ClassId == classId.Value);
                }

                // 1. Самый старший первоклассник
                var oldestFirstGraderQuery = (from s in _context.Students
                                              join c in _context.Classes on s.ClassId equals c.Id
                                              where c.Name.StartsWith("1")
                                              orderby s.BirthDate ascending
                                              select new YoungestFirstGrader
                                              {
                                                  LastName = s.LastName ?? "Не указано",
                                                  FirstName = s.FirstName ?? "Не указано",
                                                  MiddleName = s.MiddleName ?? "Не указано",
                                                  BirthDate = s.BirthDate,
                                                  ClassName = c.Name ?? "Не указано"
                                              }).FirstOrDefault();

                // Оборачиваем в список, так как YoungestFirstGraderData — это List<YoungestFirstGrader>
                var youngestFirstGraderData = oldestFirstGraderQuery != null
                    ? new List<YoungestFirstGrader> { oldestFirstGraderQuery }
                    : new List<YoungestFirstGrader>();

                // 2. Количество учеников во всех вторых классах
                var secondGradeStudentsCount = (from s in _context.Students
                                                join c in _context.Classes on s.ClassId equals c.Id
                                                where c.Name.StartsWith("2")
                                                select c).Count();

                // 3. Количество учеников у каждого классного руководителя
                var classesWithTeachers = (from c in _context.Classes
                                           join t in _context.Teachers on c.TeacherId equals t.Id into teacherGroup
                                           from t in teacherGroup.DefaultIfEmpty()
                                           select new
                                           {
                                               ClassId = c.Id,
                                               TeacherName = t != null ? t.FullName : "Не назначен"
                                           }).ToList();

                var studentsPerTeacher = (from c in classesWithTeachers
                                          join s in studentsQuery on c.ClassId equals s.ClassId into studentGroup
                                          from s in studentGroup.DefaultIfEmpty()
                                          group s by new { c.TeacherName } into g
                                          select new StudentPerTeacher
                                          {
                                              TeacherName = g.Key.TeacherName,
                                              StudentCount = g.Count(s => s != null)
                                          }).ToList();

                // Формируем отчёт
                var report = new ReportViewModel
                {
                    LastName = oldestFirstGraderQuery?.LastName ?? string.Empty,
                    FirstName = oldestFirstGraderQuery?.FirstName ?? string.Empty,
                    MiddleName = oldestFirstGraderQuery?.MiddleName ?? string.Empty,
                    BirthDate = oldestFirstGraderQuery?.BirthDate ?? DateTime.MinValue,
                    ClassName = oldestFirstGraderQuery?.ClassName ?? string.Empty,
                    TeacherName = oldestFirstGraderQuery != null
                        ? (from c in _context.Classes
                           join t in _context.Teachers on c.TeacherId equals t.Id
                           where c.Name == oldestFirstGraderQuery.ClassName
                           select t.FullName).FirstOrDefault() ?? string.Empty
                        : string.Empty,
                    StudentCount = 0,
                    YoungestFirstGraderData = youngestFirstGraderData,
                    SecondGradeStudentsCount = secondGradeStudentsCount,
                    StudentsPerTeacherData = studentsPerTeacher
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetReports: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { Message = "Произошла ошибка при получении отчёта", Details = ex.Message });
            }
        }
    }
}