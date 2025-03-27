using System;

namespace WebApplication3.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; } // Nullable DateTime
        public int ClassId { get; set; }
        public Class Class { get; set; }
    }
}