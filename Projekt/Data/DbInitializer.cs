using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projekt.Models;

namespace Projekt.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.EnsureCreatedAsync();

            if (await userManager.Users.AnyAsync())
            {
                return;
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

            // 2. Base Users (Login credentials)
            var adminUser = new IdentityUser { UserName = "admin@nexus.com", Email = "admin@nexus.com", EmailConfirmed = true, PhoneNumber = "+48 600 999 001" };
            var hrUser = new IdentityUser { UserName = "hr@nexus.com", Email = "hr@nexus.com", EmailConfirmed = true, PhoneNumber = "+48 600 999 002" };
            var managerUser = new IdentityUser { UserName = "manager@nexus.com", Email = "manager@nexus.com", EmailConfirmed = true, PhoneNumber = "+48 600 999 003" };
            var employeeUser = new IdentityUser { UserName = "employee@nexus.com", Email = "employee@nexus.com", EmailConfirmed = true, PhoneNumber = "+48 600 999 004" };

            await CreateUserIfNotExists(userManager, adminUser, "Password123!", "Admin");
            await CreateUserIfNotExists(userManager, hrUser, "Password123!", "HR");
            await CreateUserIfNotExists(userManager, managerUser, "Password123!", "Manager");
            await CreateUserIfNotExists(userManager, employeeUser, "Password123!", "Employee");

            // 3. Departments
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(new List<Department>
                {
                    new Department { Name = "IT & Engineering", Description = "Driving innovation through scalable software solutions, cloud infrastructure, and cutting-edge R&D.", ManagerId = managerUser.Id },
                    new Department { Name = "Human Resources", Description = "Fostering a world-class workplace culture, managing talent acquisition, and employee development.", ManagerId = hrUser.Id },
                    new Department { Name = "Global Sales", Description = "Expanding market presence and building lasting relationships with enterprise clients worldwide.", ManagerId = managerUser.Id },
                    new Department { Name = "Marketing & Brand", Description = "Crafting compelling narratives and digital experiences to elevate our brand identity.", ManagerId = managerUser.Id },
                    new Department { Name = "Finance & Legal", Description = "Ensuring fiscal responsibility, compliance, and strategic financial planning.", ManagerId = managerUser.Id },
                    new Department { Name = "Customer Success", Description = "Dedicated to ensuring our clients achieve their business goals using our products.", ManagerId = managerUser.Id }
                });
                await context.SaveChangesAsync();
            }

            var itDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT & Engineering");
            var hrDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Human Resources");
            var salesDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Global Sales");
            var mktDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Marketing & Brand");
            var finDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Finance & Legal");
            var csDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Customer Success");

            // 4. Employees (Rich Data Set)
            if (!context.Employees.Any())
            {
                var employees = new List<Employee>
                {
                    // Org Leaders
                    new Employee { FirstName = "Admin", LastName = "System", Email = "admin@nexus.com", PhoneNumber = "+48 600 999 001", Position = "CTO", Salary = 25000, HireDate = DateTime.Now.AddYears(-5), DepartmentId = itDept?.Id, UserId = adminUser.Id },
                    new Employee { FirstName = "Sarah", LastName = "Connors", Email = "hr@nexus.com", PhoneNumber = "+48 600 999 002", Position = "VP of People", Salary = 18000, HireDate = DateTime.Now.AddYears(-4), DepartmentId = hrDept?.Id, UserId = hrUser.Id },
                    new Employee { FirstName = "Mike", LastName = "Ross", Email = "manager@nexus.com", PhoneNumber = "+48 600 999 003", Position = "Sales Director", Salary = 22000, HireDate = DateTime.Now.AddYears(-3), DepartmentId = salesDept?.Id, UserId = managerUser.Id },

                    // IT Team
                    new Employee { FirstName = "John", LastName = "Dev", Email = "employee@nexus.com", PhoneNumber = "+48 600 999 004", Position = "Senior Backend Engineer", Salary = 15000, HireDate = DateTime.Now.AddYears(-2), DepartmentId = itDept?.Id, UserId = employeeUser.Id },
                    new Employee { FirstName = "Alice", LastName = "Chen", Email = "alice.chen@nexus.com", PhoneNumber = "+48 601 123 456", Position = "Lead Frontend Developer", Salary = 16000, HireDate = DateTime.Now.AddMonths(-18), DepartmentId = itDept?.Id },
                    new Employee { FirstName = "David", LastName = "Kim", Email = "david.kim@nexus.com", PhoneNumber = "+48 601 234 567", Position = "DevOps Engineer", Salary = 14500, HireDate = DateTime.Now.AddMonths(-10), DepartmentId = itDept?.Id },
                    new Employee { FirstName = "Emma", LastName = "Watson", Email = "emma.watson@nexus.com", PhoneNumber = "+48 601 345 678", Position = "QA Specialist", Salary = 9000, HireDate = DateTime.Now.AddMonths(-5), DepartmentId = itDept?.Id },
                    new Employee { FirstName = "Robert", LastName = "Hackerman", Email = "r.hackerman@nexus.com", PhoneNumber = "+48 601 456 789", Position = "Security Analyst", Salary = 15500, HireDate = DateTime.Now.AddYears(-1), DepartmentId = itDept?.Id },

                    // Sales Team
                    new Employee { FirstName = "James", LastName = "Bond", Email = "j.bond@nexus.com", PhoneNumber = "+48 700 007 007", Position = "Enterprise Account Executive", Salary = 12000, HireDate = DateTime.Now.AddYears(-6), DepartmentId = salesDept?.Id },
                    new Employee { FirstName = "Linda", LastName = "Sales", Email = "linda.sales@nexus.com", PhoneNumber = "+48 700 111 222", Position = "Sales Representative", Salary = 8500, HireDate = DateTime.Now.AddMonths(-8), DepartmentId = salesDept?.Id },
                    new Employee { FirstName = "Tom", LastName = "Crunch", Email = "tom.crunch@nexus.com", PhoneNumber = "+48 700 333 444", Position = "Sales Development Rep", Salary = 6500, HireDate = DateTime.Now.AddMonths(-2), DepartmentId = salesDept?.Id },

                    // Marketing
                    new Employee { FirstName = "Emily", LastName = "Paris", Email = "emily.p@nexus.com", PhoneNumber = "+48 500 123 123", Position = "Social Media Manager", Salary = 9500, HireDate = DateTime.Now.AddMonths(-14), DepartmentId = mktDept?.Id },
                    new Employee { FirstName = "Lucas", LastName = "Art", Email = "lucas.art@nexus.com", PhoneNumber = "+48 500 999 888", Position = "Graphic Designer", Salary = 8000, HireDate = DateTime.Now.AddMonths(-4), DepartmentId = mktDept?.Id },

                    // Finance
                    new Employee { FirstName = "Oscar", LastName = "Numbers", Email = "oscar.n@nexus.com", PhoneNumber = "+48 800 555 666", Position = "Financial Analyst", Salary = 11000, HireDate = DateTime.Now.AddYears(-2), DepartmentId = finDept?.Id },
                    
                    // Customer Success
                    new Employee { FirstName = "Julia", LastName = "Help", Email = "julia.h@nexus.com", PhoneNumber = "+48 900 111 000", Position = "CS Manager", Salary = 10500, HireDate = DateTime.Now.AddYears(-1), DepartmentId = csDept?.Id },
                    new Employee { FirstName = "Mark", LastName = "Support", Email = "mark.s@nexus.com", PhoneNumber = "+48 900 222 333", Position = "Support Specialist", Salary = 6000, HireDate = DateTime.Now.AddMonths(-3), DepartmentId = csDept?.Id }
                };

                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();
            }

            // 5. Projects
            if (!context.Projects.Any())
            {
                context.Projects.AddRange(new List<Project>
                {
                    new Project { Name = "Project Phoenix", Description = "Complete rewrite of the legacy monolith into microservices.", StartDate = DateTime.Now.AddMonths(-6), EndDate = DateTime.Now.AddMonths(2), ManagerId = managerUser.Id },
                    new Project { Name = "Mobile App v2.0", Description = "Adding dark mode and offline sync to the iOS/Android apps.", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(4), ManagerId = managerUser.Id },
                    new Project { Name = "Cloud Migration 2026", Description = "Moving primary data centers to AWS/Azure hybrid cloud.", StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(12), ManagerId = managerUser.Id },
                    new Project { Name = "Q1 Marketing Campaign", Description = "Viral social media push for the new product launch.", StartDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(2), ManagerId = managerUser.Id },
                    new Project { Name = "Internal Audit", Description = "Annual financial and security compliance audit.", StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddDays(-5), ManagerId = managerUser.Id }, // Ended
                    new Project { Name = "AI Integration Pilot", Description = "Testing LLM integration for customer support automation.", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6), ManagerId = managerUser.Id }
                });
                await context.SaveChangesAsync();
            }

            // 6. Assign Employees to Projects (Many-to-Many)
            if (!context.EmployeeProjects.Any())
            {
                var projects = await context.Projects.ToListAsync();
                var empList = await context.Employees.ToListAsync();
                var rnd = new Random();

                foreach (var project in projects)
                {
                    // Assign random 3-8 employees to each project
                    var randomEmployees = empList.OrderBy(x => rnd.Next()).Take(rnd.Next(3, 8)).ToList();
                    
                    foreach (var emp in randomEmployees)
                    {
                        if (!context.EmployeeProjects.Any(ep => ep.EmployeeId == emp.Id && ep.ProjectId == project.Id))
                        {
                            context.EmployeeProjects.Add(new EmployeeProject 
                            { 
                                EmployeeId = emp.Id, 
                                ProjectId = project.Id, 
                                AssignedDate = DateTime.Now.AddDays(-rnd.Next(1, 100)) 
                            });
                        }
                    }
                }
                await context.SaveChangesAsync();
            }

            // 7. Leave Requests (Rich History)
            if (!context.LeaveRequests.Any())
            {
                var empList = await context.Employees.ToListAsync();
                var rnd = new Random();
                var requestTypes = new[] { "Vacation", "Sick Leave", "Remote Work", "Personal", "Unpaid" };
                var statuses = new[] { LeaveRequestStatus.Approved, LeaveRequestStatus.Rejected, LeaveRequestStatus.Pending };

                foreach (var emp in empList)
                {
                    // Create 1-4 requests per employee
                    int numRequests = rnd.Next(1, 5);
                    for (int i = 0; i < numRequests; i++)
                    {
                        var status = statuses[rnd.Next(statuses.Length)];
                        var start = DateTime.Now.AddDays(rnd.Next(-100, 60)); // Some in past, some in future
                        var end = start.AddDays(rnd.Next(1, 14));

                        var req = new LeaveRequest
                        {
                            EmployeeId = emp.Id,
                            StartDate = start,
                            EndDate = end,
                            LeaveType = requestTypes[rnd.Next(requestTypes.Length)],
                            Reason = "Generated mock request for testing purposes.",
                            Status = status,
                            RequestDate = start.AddDays(-10)
                        };

                        if (status == LeaveRequestStatus.Approved)
                        {
                            req.ApprovedByUserId = managerUser.Id;
                            req.ApprovedDate = req.RequestDate.AddDays(2);
                            req.Comments = "Have fun!";
                        }
                        else if (status == LeaveRequestStatus.Rejected)
                        {
                            req.ApprovedByUserId = hrUser.Id;
                            req.ApprovedDate = req.RequestDate.AddDays(1);
                            req.Comments = "Critical project phase, cannot approve.";
                        }

                        context.LeaveRequests.Add(req);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUserIfNotExists(UserManager<IdentityUser> userManager, IdentityUser user, string password, string role)
        {
            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
