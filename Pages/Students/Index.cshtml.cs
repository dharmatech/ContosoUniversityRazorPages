using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public enum SortOrder
    {
        NameAsc,
        NameDsc,
        DateAsc,
        DateDsc,
        None
    }

    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public SortOrder NameSort { get; set; }
        public SortOrder DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<Student> Students { get;set; }
                
        public async Task OnGetAsync(SortOrder sortOrder, string searchString)
        {            
            NameSort = sortOrder == SortOrder.NameAsc ? SortOrder.NameDsc : SortOrder.NameAsc;
            DateSort = sortOrder == SortOrder.DateAsc ? SortOrder.DateDsc : SortOrder.DateAsc;

            if (sortOrder == SortOrder.None) NameSort = SortOrder.NameDsc;

            CurrentFilter = searchString;

            var students = _context.Students.Select(student => student);
                        
            if (!String.IsNullOrEmpty(searchString)) 
                students = students.Where(student => 
                    student.LastName.Contains(searchString) || 
                    student.FirstMidName.Contains(searchString));

            if      (sortOrder == SortOrder.NameAsc) students = students.OrderBy(          student => student.LastName);
            else if (sortOrder == SortOrder.NameDsc) students = students.OrderByDescending(student => student.LastName);
            else if (sortOrder == SortOrder.DateAsc) students = students.OrderBy(          student => student.EnrollmentDate);
            else if (sortOrder == SortOrder.DateDsc) students = students.OrderByDescending(student => student.EnrollmentDate);
            else                                     students = students.OrderBy(          student => student.LastName);

            Students = await students.AsNoTracking().ToListAsync();
        }
    }
}
