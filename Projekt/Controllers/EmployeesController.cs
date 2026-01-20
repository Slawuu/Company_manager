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
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, int? departmentId, string position)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["DepartmentFilter"] = departmentId;
            ViewData["PositionFilter"] = position;

            var employees = from e in _context.Employees.Include(e => e.Department)
                            select e;

            if (!User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Manager"))
            {
                var currentUserEmail = User.Identity?.Name;
                var currentEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == currentUserEmail);
                
                if (currentEmployee != null && currentEmployee.DepartmentId.HasValue)
                {
                    employees = employees.Where(e => e.DepartmentId == currentEmployee.DepartmentId);
                    ViewBag.IsRestrictedView = true;
                    ViewBag.CurrentDepartmentName = currentEmployee.Department?.Name;
                    
                    ViewBag.Positions = await _context.Employees
                        .Where(e => e.DepartmentId == currentEmployee.DepartmentId)
                        .Select(e => e.Position)
                        .Distinct()
                        .ToListAsync();
                }
                else
                {
                    // User has no department assigned
                    ViewBag.IsRestrictedView = true;
                    ViewBag.NoDepartmentAssigned = true;
                    ViewBag.Positions = new List<string>();
                    employees = employees.Where(e => false); // Return no employees
                }
            }
            else
            {
                ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
                ViewBag.Positions = await _context.Employees.Select(e => e.Position).Distinct().ToListAsync();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.LastName.Contains(searchString) || e.FirstName.Contains(searchString));
            }

            if (departmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId);
            }

            if (!string.IsNullOrEmpty(position))
            {
                employees = employees.Where(e => e.Position == position);
            }

            var employeeList = await employees.OrderBy(e => e.LastName).ToListAsync();
            
            var employeesWithRoles = new List<EmployeeWithRoleViewModel>();
            foreach (var employee in employeeList)
            {
                string? role = null;
                if (!string.IsNullOrEmpty(employee.UserId))
                {
                    var user = await _userManager.FindByIdAsync(employee.UserId);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        role = roles.FirstOrDefault();
                    }
                }
                
                employeesWithRoles.Add(new EmployeeWithRoleViewModel
                {
                    Employee = employee,
                    Role = role
                });
            }

            return View(employeesWithRoles);
        }

        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.EmployeeProjects)
                    .ThenInclude(ep => ep.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }


        [Authorize(Roles = "Admin,HR")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR", "Admin" });
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    EmailConfirmed = true
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    
                    var employee = new Employee
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        HireDate = model.HireDate,
                        Position = model.Position,
                        Salary = model.Salary,
                        DepartmentId = model.DepartmentId,
                        UserId = user.Id
                    };
                    
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Employee created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["Error"] = "Failed to create employee. Check validation errors.";
            }
            
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
            ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR" }, model.Role);
            return View(model);
        }


        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            
            var model = new EditEmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                HireDate = employee.HireDate,
                Position = employee.Position,
                Salary = employee.Salary,
                DepartmentId = employee.DepartmentId,
                UserId = employee.UserId
            };

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            
            string? currentRole = null;
            if (!string.IsNullOrEmpty(employee.UserId))
            {
                var user = await _userManager.FindByIdAsync(employee.UserId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    currentRole = roles.FirstOrDefault();
                }
            }
            model.Role = currentRole;
            
            ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR", "Admin" }, currentRole);
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for unique Email
                if (await _context.Employees.AnyAsync(e => e.Email == model.Email && e.Id != model.Id))
                {
                    ModelState.AddModelError("Email", "Ten adres Email jest już zajęty przez innego pracownika.");
                }

                // Check for unique PhoneNumber
                if (!string.IsNullOrEmpty(model.PhoneNumber) && await _context.Employees.AnyAsync(e => e.PhoneNumber == model.PhoneNumber && e.Id != model.Id))
                {
                    ModelState.AddModelError("PhoneNumber", "Ten numer telefonu jest już zajęty przez innego pracownika.");
                }

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Validation failed. Email or Phone Number already exists.";
                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
                    ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR", "Admin" }, model.Role);
                    return View(model);
                }

                try
                {
                    var employee = await _context.Employees.FindAsync(id);
                    if (employee == null)
                    {
                        return NotFound();
                    }

                    // Update Employee properties
                    employee.FirstName = model.FirstName;
                    employee.LastName = model.LastName;
                    employee.Email = model.Email;
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.HireDate = model.HireDate;
                    employee.Position = model.Position;
                    employee.Salary = model.Salary;
                    employee.DepartmentId = model.DepartmentId;

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    
                    if (!string.IsNullOrEmpty(employee.UserId))
                    {
                        var user = await _userManager.FindByIdAsync(employee.UserId);
                        if (user != null)
                        {
                            // Update Email/Username if changed
                            if (user.Email != employee.Email)
                            {
                                user.Email = employee.Email;
                                user.UserName = employee.Email;
                                await _userManager.UpdateAsync(user);
                            }

                            // Update Password if provided
                            if (!string.IsNullOrEmpty(model.Password))
                            {
                                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                                if (!result.Succeeded)
                                {
                                    TempData["Error"] = "Failed to update password.";
                                    foreach (var error in result.Errors)
                                    {
                                        ModelState.AddModelError(string.Empty, error.Description);
                                    }
                                    // Reload data for view
                                    ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
                                    ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR", "Admin" }, model.Role);
                                    return View(model);
                                }
                            }
                            
                            // Update Role
                            if (!string.IsNullOrEmpty(model.Role))
                            {
                                var currentRoles = await _userManager.GetRolesAsync(user);
                                if (!currentRoles.Contains(model.Role))
                                {
                                    if (currentRoles.Any())
                                    {
                                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                                    }
                                    await _userManager.AddToRoleAsync(user, model.Role);
                                }
                            }
                        }
                    }
                    
                    TempData["Success"] = "Employee updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(model.Id))
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
            else 
            {
                 TempData["Error"] = "Validation failed. Please check the form.";
            }
            
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", model.DepartmentId);
            ViewData["Roles"] = new SelectList(new[] { "Employee", "Manager", "HR", "Admin" }, model.Role);
            return View(model);
        }


        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
