﻿using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Range(1,12)]
        public int ForGrade { get; set; }
        
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
