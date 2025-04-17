using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly SchoolDbContext _context;

        public AccountController(SchoolDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Проверяем, существует ли пользователь с таким логином и паролем
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == username && u.Password == password);

            if (user != null)
            {
                // Устанавливаем состояние аутентификации
                Startup.IsAuthenticated = true;
                return RedirectToAction("Index", "Students");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View();
        }

        public IActionResult Logout()
        {
            // Сбрасываем состояние аутентификации
            Startup.IsAuthenticated = false;
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}