using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorIndexData { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        public async Task OnGetAsync(int? id, int? courseID)
        {
            InstructorIndexData = new InstructorIndexData();

            InstructorIndexData.Instructors = await _context.Instructors
                .Include(instructor => instructor.OfficeAssignment)
                .Include(instructor => instructor.Courses).ThenInclude(course => course.Department)
                .OrderBy(instructor => instructor.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorID = id.Value;
                var instructor = InstructorIndexData.Instructors
                    .Where(instructor => instructor.ID == id.Value)
                    .Single();
                
                InstructorIndexData.Courses = instructor.Courses;
            }

            if (courseID != null)
            {
                CourseID = courseID.Value;

                var selectedCourse = InstructorIndexData.Courses
                    .Where(course => course.CourseID == courseID.Value).Single();

                await _context.Entry(selectedCourse)
                    .Collection(course => course.Enrollments)
                    .LoadAsync();

                foreach (var enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(enrollment => enrollment.Student).LoadAsync();
                }

                InstructorIndexData.Enrollments = selectedCourse.Enrollments;
            }
        }
    }
}
