using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projekt.Models;

namespace Projekt.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if users already exist
            if (await userManager.Users.AnyAsync())
            {
                return; // DB has been seeded
            }

            // 1. Roles
            string[] roles = { "Admin", "HR", "Manager", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Users and Roles
            var adminUser = new IdentityUser { UserName = "admin@nexus.com", Email = "admin@nexus.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                await userManager.CreateAsync(adminUser, "Password123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var hrUser = new IdentityUser { UserName = "hr@nexus.com", Email = "hr@nexus.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(hrUser.Email) == null)
            {
                await userManager.CreateAsync(hrUser, "Password123!");
                await userManager.AddToRoleAsync(hrUser, "HR");
            }

            var managerUser = new IdentityUser { UserName = "manager@nexus.com", Email = "manager@nexus.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(managerUser.Email) == null)
            {
                await userManager.CreateAsync(managerUser, "Password123!");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }

            var employeeUser = new IdentityUser { UserName = "employee@nexus.com", Email = "employee@nexus.com", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(employeeUser.Email) == null)
            {
                await userManager.CreateAsync(employeeUser, "Password123!");
                await userManager.AddToRoleAsync(employeeUser, "Employee");
            }

            // 3. Departments
            if (!context.Departments.Any())
            {
                var departments = new List<Department>
                {
                    new Department { Name = "IT", Description = "Information Technology and Development", ManagerId = managerUser.Id },
                    new Department { Name = "HR", Description = "Human Resources and Talent Acquisition", ManagerId = hrUser.Id },
                    new Department { Name = "Sales", Description = "Global Sales and Account Management", ManagerId = managerUser.Id },
                    new Department { Name = "Marketing", Description = "Brand, Digital Marketing and Events", ManagerId = managerUser.Id }
                };

                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }

            // 4. Employees
            if (!context.Employees.Any())
            {
                var itDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT");
                var hrDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "HR");
                var salesDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Sales");

                var employees = new List<Employee>
                {
                    new Employee { FirstName = "Admin", LastName = "User", Email = "admin@nexus.com", Position = "System Administrator", Salary = 15000, HireDate = DateTime.Now.AddYears(-2), DepartmentId = itDept?.Id, UserId = adminUser.Id },
                    new Employee { FirstName = "HR", LastName = "Manager", Email = "hr@nexus.com", Position = "HR Director", Salary = 12000, HireDate = DateTime.Now.AddYears(-1), DepartmentId = hrDept?.Id, UserId = hrUser.Id },
                    new Employee { FirstName = "Project", LastName = "Manager", Email = "manager@nexus.com", Position = "Senior Project Manager", Salary = 14000, HireDate = DateTime.Now.AddMonths(-6), DepartmentId = itDept?.Id, UserId = managerUser.Id },
                    new Employee { FirstName = "John", LastName = "Doe", Email = "employee@nexus.com", Position = "Software Developer", Salary = 8000, HireDate = DateTime.Now.AddMonths(-3), DepartmentId = itDept?.Id, UserId = employeeUser.Id },
                    new Employee { FirstName = "Alice", LastName = "Smith", Email = "alice.smith@nexus.com", Position = "UX Designer", Salary = 9000, HireDate = DateTime.Now.AddMonths(-4), DepartmentId = itDept?.Id },
                    new Employee { FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@nexus.com", Position = "Sales Representative", Salary = 7000, HireDate = DateTime.Now.AddMonths(-5), DepartmentId = salesDept?.Id }
                };

                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();
            }

            // 5. Projects
            if (!context.Projects.Any())
            {
                var projects = new List<Project>
                {
                    new Project { Name = "Project Phoenix", Description = "Revamping the legacy system with Neumorphic UI.", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(4), ManagerId = managerUser.Id },
                    new Project { Name = "Nexus Mobile App", Description = "Developing the iOS and Android applications.", StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(6), ManagerId = managerUser.Id },
                    new Project { Name = "Cloud Migration", Description = "Migrating on-premise servers to Azure.", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(12), ManagerId = managerUser.Id }
                };

                context.Projects.AddRange(projects);
                await context.SaveChangesAsync();
            }

            // 6. Assign Employees to Projects
            if (!context.EmployeeProjects.Any())
            {
                var projectPhoenix = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Project Phoenix");
                var nexusApp = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Nexus Mobile App");

                var devEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Email == "employee@nexus.com");
                var uxEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Email == "alice.smith@nexus.com");
                var managerEmp = await context.Employees.FirstOrDefaultAsync(e => e.Email == "manager@nexus.com");

                if (projectPhoenix != null && devEmployee != null)
                {
                    context.EmployeeProjects.Add(new EmployeeProject { EmployeeId = devEmployee.Id, ProjectId = projectPhoenix.Id });
                }
                if (projectPhoenix != null && uxEmployee != null)
                {
                    context.EmployeeProjects.Add(new EmployeeProject { EmployeeId = uxEmployee.Id, ProjectId = projectPhoenix.Id });
                }
                if (nexusApp != null && managerEmp != null)
                {
                    context.EmployeeProjects.Add(new EmployeeProject { EmployeeId = managerEmp.Id, ProjectId = nexusApp.Id });
                }

                await context.SaveChangesAsync();
            }

            // 7. Leave Requests
            if (!context.LeaveRequests.Any())
            {
                var devEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Email == "employee@nexus.com");
                var salesEmployee = await context.Employees.FirstOrDefaultAsync(e => e.Email == "bob.johnson@nexus.com");

                if (devEmployee != null)
                {
                    context.LeaveRequests.Add(new LeaveRequest
                    {
                        EmployeeId = devEmployee.Id,
                        StartDate = DateTime.Now.AddDays(10),
                        EndDate = DateTime.Now.AddDays(15),
                        LeaveType = "Vacation",
                        Reason = "Summer holiday",
                        Status = LeaveRequestStatus.Pending,
                        RequestDate = DateTime.Now
                    });
                }

                if (salesEmployee != null)
                {
                    context.LeaveRequests.Add(new LeaveRequest
                    {
                        EmployeeId = salesEmployee.Id,
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now.AddDays(-2),
                        LeaveType = "Sick Leave",
                        Reason = "Flu",
                        Status = LeaveRequestStatus.Approved,
                        ApprovedByUserId = managerUser.Id,
                        ApprovedDate = DateTime.Now.AddDays(-6),
                        RequestDate = DateTime.Now.AddDays(-10)
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
