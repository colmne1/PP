using System;
using System.Collections.Generic;

namespace WebApplication3.Models
{
    public class ReportViewModel
    {
        public class YoungestFirstGrader
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public DateTime BirthDate { get; set; }
            public string ClassName { get; set; }
        }

        public class StudentsPerTeacher
        {
            public string TeacherName { get; set; }
            public int StudentCount { get; set; }
        }

        public YoungestFirstGrader YoungestFirstGraderData { get; set; }
        public int SecondGradeStudentsCount { get; set; }
        public List<StudentsPerTeacher> StudentsPerTeacherData { get; set; }
    }
}