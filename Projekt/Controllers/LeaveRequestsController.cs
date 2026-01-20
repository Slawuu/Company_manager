using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LeaveRequestsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            IQueryable<LeaveRequest> leaveRequests;

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                leaveRequests = _context.LeaveRequests.Include(l => l.Employee);
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
                if (employee == null)
                {
                    return View(new List<LeaveRequest>());
                }
                leaveRequests = _context.LeaveRequests
                    .Include(l => l.Employee)
                    .Where(l => l.EmployeeId == employee.Id);
            }

            return View(await leaveRequests.OrderByDescending(l => l.RequestDate).ToListAsync());
        }


        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null && !User.IsInRole("Admin") && !User.IsInRole("HR"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (User.IsInRole("Admin") || User.IsInRole("HR"))
            {
                var employees = await _context.Employees.ToListAsync();
                ViewData["EmployeeId"] = new SelectList(employees.Select(e => new
                {
                    Id = e.Id,
                    FullName = $"{e.FirstName} {e.LastName}"
                }), "Id", "FullName");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,StartDate,EndDate,LeaveType,Reason")] LeaveRequest leaveRequest)
        {
            ModelState.Remove("Employee");
            
            if (!User.IsInRole("Admin") && !User.IsInRole("HR"))
            {
                var userId = _userManager.GetUserId(User);
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
                if (employee != null)
                {
                    leaveRequest.EmployeeId = employee.Id;
                    ModelState.Remove("EmployeeId");
                }
                else
                {
                    ModelState.AddModelError("", "Employee record not found for current user.");
                    return View(leaveRequest);
                }
            }

            if (ModelState.IsValid)
            {
                leaveRequest.RequestDate = DateTime.Now;
                leaveRequest.Status = LeaveRequestStatus.Pending;
                
                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole("Admin") || User.IsInRole("HR"))
            {
                var employees = await _context.Employees.ToListAsync();
                ViewData["EmployeeId"] = new SelectList(employees.Select(e => new
                {
                    Id = e.Id,
                    FullName = $"{e.FirstName} {e.LastName}"
                }), "Id", "FullName", leaveRequest.EmployeeId);
            }
            return View(leaveRequest);
        }


        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }


        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ApproveConfirmed(int id, string comments)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = LeaveRequestStatus.Approved;
                leaveRequest.ApprovedByUserId = _userManager.GetUserId(User);
                leaveRequest.ApprovedDate = DateTime.Now;
                leaveRequest.Comments = comments;
                
                _context.Update(leaveRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reject(int id, string comments)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                leaveRequest.Status = LeaveRequestStatus.Rejected;
                leaveRequest.ApprovedByUserId = _userManager.GetUserId(User);
                leaveRequest.ApprovedDate = DateTime.Now;
                leaveRequest.Comments = comments;
                
                _context.Update(leaveRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
