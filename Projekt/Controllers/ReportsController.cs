using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> EmployeesByDepartment()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.Department != null)
                .ToListAsync();

            var grouped = employees
                .GroupBy(e => e.Department!.Name)
                .OrderBy(g => g.Key);

            return View(grouped);
        }


        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> HiredInPeriod(DateTime? startDate, DateTime? endDate)
        {
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            if (!startDate.HasValue && !endDate.HasValue)
            {
                startDate = DateTime.Now.AddMonths(-6);
                endDate = DateTime.Now;
            }

            var query = _context.Employees.Include(e => e.Department).AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(e => e.HireDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.HireDate <= endDate);
            }

            var employees = await query
                .OrderByDescending(e => e.HireDate)
                .ToListAsync();

            return View(employees);
        }


        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> ProjectsSummary()
        {
            var projects = await _context.Projects
                .Include(p => p.EmployeeProjects)
                    .ThenInclude(ep => ep.Employee)
                .ToListAsync();

            return View(projects);
        }


        public async Task<IActionResult> LeaveRequestsSummary(int? year)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            ViewBag.Year = year;

            var query = _context.LeaveRequests
                .Include(l => l.Employee)
                .Where(l => l.StartDate.Year == year || l.EndDate.Year == year);

            if (!User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == currentUser.Id);
                    if (employee != null)
                    {
                        query = query.Where(l => l.EmployeeId == employee.Id);
                    }
                    else
                    {
                         // If regular user has no linked employee record, show nothing
                         query = query.Where(l => false);
                    }
                }
            }

            var leaveRequests = await query
                .OrderBy(l => l.Status)
                .ThenByDescending(l => l.RequestDate)
                .ToListAsync();

            return View(leaveRequests);
        }
    }
}
