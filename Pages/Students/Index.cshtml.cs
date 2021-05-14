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

    public class OnGetAsyncParameters
    {
        public SortOrder? SortOrder { get; set; }
        public string CurrentFilter { get; set; }
        public string SearchString { get; set; }
        public int? PageIndex { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();

            if (SortOrder     != null) dict.Add("SortOrder",     SortOrder.ToString());
            if (CurrentFilter != null) dict.Add("CurrentFilter", CurrentFilter);
            if (SearchString  != null) dict.Add("SearchString",  SearchString);
            if (PageIndex     != null) dict.Add("PageIndex",     PageIndex.ToString());

            return dict;
        }
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
                
        public async Task OnGetAsync(OnGetAsyncParameters onGetAsyncParameters)
        {
            var sortOrder = onGetAsyncParameters.SortOrder;
            var currentFilter = onGetAsyncParameters.CurrentFilter;
            var searchString = onGetAsyncParameters.SearchString;
            var pageIndex = onGetAsyncParameters.PageIndex;

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
