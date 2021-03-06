using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration Configuration;

        public IndexModel(SchoolContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public SortOrder NameSort { get; set; }
        public SortOrder DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public SortOrder? CurrentSort { get; set; }

        public PaginatedList<Student> Students { get;set; }
                
        public async Task OnGetAsync(SortOrder? sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;

            NameSort = sortOrder == SortOrder.NameAsc ? SortOrder.NameDsc : SortOrder.NameAsc;
            DateSort = sortOrder == SortOrder.DateAsc ? SortOrder.DateDsc : SortOrder.DateAsc;
            
            if (sortOrder == SortOrder.None) NameSort = SortOrder.NameDsc;
            if (sortOrder == null)           NameSort = SortOrder.NameDsc;

            if (searchString == null)
                searchString = currentFilter;
            else
                pageIndex = 1;

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

            var pageSize = Configuration.GetValue("PageSize", 4);

            Students = await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
