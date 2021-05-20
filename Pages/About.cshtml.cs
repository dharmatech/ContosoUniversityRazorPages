using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages
{
    public class AboutModel : PageModel
    {
        private readonly SchoolContext _context;

        public AboutModel(SchoolContext context) => _context = context;

        public IList<EnrollmentDateGroup> Students { get; set; }

        public async Task OnGetAsync()
        {
            //var data =
            //    from student in _context.Students
            //    group student by student.EnrollmentDate into date_group
            //    select new EnrollmentDateGroup()
            //    {
            //        EnrollmentDate = date_group.Key,
            //        StudentCount = date_group.Count()
            //    };

            var data = _context.Students
                .GroupBy(student => student.EnrollmentDate)
                .Select(grouping => new EnrollmentDateGroup()
                {
                    EnrollmentDate = grouping.Key,
                    StudentCount = grouping.Count()
                });

            Students = await data.AsNoTracking().ToListAsync();
        }
    }
}
