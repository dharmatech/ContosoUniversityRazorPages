using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public CreateModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var empty_student = new Student();

            if (await TryUpdateModelAsync<Student>(
                empty_student, 
                "student", 
                student => student.FirstMidName, 
                student => student.LastName, 
                student => student.EnrollmentDate))
            {
                _context.Students.Add(empty_student);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
                        
            return Page();
        }
    }
}
