using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public ClassesController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: api/classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
        {
            var classes = await _context.Classes
                .Include(c => c.Teacher)
                .ToListAsync();
            return Ok(classes);
        }

        // POST: api/classes/assign-teacher
        [HttpPost("assign-teacher")]
        public async Task<IActionResult> AssignTeacher(int classId, int teacherId)
        {
            var classToAssign = await _context.Classes.FindAsync(classId);
            if (classToAssign == null)
            {
                return NotFound(new { Message = "Класс не найден" });
            }

            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                return BadRequest(new { Message = "Преподаватель не найден" });
            }

            try
            {
                classToAssign.TeacherId = teacherId;
                _context.Update(classToAssign);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Учитель успешно назначен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Ошибка: {ex.Message}" });
            }
        }
    }
}