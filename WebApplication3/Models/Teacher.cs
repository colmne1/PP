using System.Collections.Generic;

namespace WebApplication3.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FullName { get; set; } // Оставляем только FullName, убираем FirstName, LastName, MiddleName
    }
}