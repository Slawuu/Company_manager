using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .Include(e => e.LeaveRequests)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }

            var user = await _userManager.FindByIdAsync(userId!);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRole = roles.FirstOrDefault();
            }

            return View(employee);
        }


        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }

            var viewModel = new EditProfileViewModel
            {
                Id = employee.Id,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check for uniqueness
            if (await _context.Employees.AnyAsync(e => e.Email == model.Email && e.Id != model.Id))
            {
                ModelState.AddModelError("Email", "Ten adres Email jest już zajęty przez innego pracownika.");
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber) && await _context.Employees.AnyAsync(e => e.PhoneNumber == model.PhoneNumber && e.Id != model.Id))
            {
                ModelState.AddModelError("PhoneNumber", "Ten numer telefonu jest już zajęty przez innego pracownika.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var currentEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (currentEmployee == null)
            {
                return NotFound();
            }

            if (model.Id != currentEmployee.Id)
            {
                return BadRequest();
            }

            currentEmployee.Email = model.Email;
            currentEmployee.PhoneNumber = model.PhoneNumber;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user != null && user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
            }

            try
            {
                _context.Update(currentEmployee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(currentEmployee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
