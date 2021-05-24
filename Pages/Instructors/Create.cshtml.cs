using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Instructors
{
    public class CreateModel : InstructorCoursesPageModel
    {
        private readonly SchoolContext _context;
        private readonly ILogger<InstructorCoursesPageModel> _logger;

        public CreateModel(SchoolContext context, ILogger<InstructorCoursesPageModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var instructor = new Instructor();

            instructor.Courses = new List<Course>();

            PopulateAssignedCourseData(_context, instructor);

            return Page();
        }

        [BindProperty]
        public Instructor Instructor { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
        {
            var newInstructor = new Instructor();

            if (selectedCourses.Length > 0)
            {
                newInstructor.Courses = new List<Course>();

                _context.Courses.Load();
            }

            foreach (var course in selectedCourses)
            {
                var foundCourse = await _context.Courses.FindAsync(int.Parse(course));

                if (foundCourse != null)
                {
                    newInstructor.Courses.Add(foundCourse);
                }
                else
                {
                    _logger.LogWarning("Course {course} not found", course);
                }
            }

            try
            {
                if (await TryUpdateModelAsync<Instructor>(newInstructor, "Instructor",
                    instructor => instructor.FirstMidName,
                    instructor => instructor.LastName,
                    instructor => instructor.HireDate,
                    instructor => instructor.OfficeAssignment))
                {
                    _context.Instructors.Add(newInstructor);

                    await _context.SaveChangesAsync();

                    return RedirectToPage("./Index");
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            PopulateAssignedCourseData(_context, Instructor);

            return Page();
        }
    }
}
