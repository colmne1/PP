using System.Collections.Generic;
using System;

namespace WebApplication3.Models
{
    public class ReportViewModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public int StudentCount { get; set; }
        public List<YoungestFirstGrader> YoungestFirstGraderData { get; set; }
        public int SecondGradeStudentsCount { get; set; }
        public List<StudentPerTeacher> StudentsPerTeacherData { get; set; }
    }

    public class YoungestFirstGrader
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ClassName { get; set; }
    }

    public class StudentPerTeacher
    {
        public string TeacherName { get; set; }
        public int StudentCount { get; set; }
    }
}