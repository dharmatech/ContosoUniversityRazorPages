using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses
{
    public class EditModel : DepartmentNamePageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public EditModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            
            Course = await _context.Courses
                .Include(c => c.Department).FirstOrDefaultAsync(m => m.CourseID == id);

            if (Course == null) return NotFound();

            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");

            PopulateDepartmentsDropDownList(_context, Course.DepartmentID);
            
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);

            if (course == null) return NotFound();

            if (await TryUpdateModelAsync<Course>(course, "course",
                course => course.Credits,
                course => course.DepartmentID,
                course => course.Title))
            {
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            PopulateDepartmentsDropDownList(_context, course.DepartmentID);

            return Page();
        }

        //private bool CourseExists(int id)
        //{
        //    return _context.Courses.Any(e => e.CourseID == id);
        //}
    }
}
