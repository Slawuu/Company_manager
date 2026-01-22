# Nexus - Human Resource Management System

Nexus is a robust enterprise-grade application for managing projects, employees, and departments. Built with ASP.NET Core MVC, it features a modern Neumorphic UI design and a comprehensive dashboard for real-time analytics.

## Features

*   **Employee Management**: Track employment details, roles, and assignments.
*   **Project Hub**: Manage project lifecycles, assign teams, and monitor progress.
*   **Leave Requests**: Automated approval workflow for vacation and sick leave.
*   **Departments**: Organize company structure effectively.
*   **Analytics**: Real-time reporting and meaningful insights.
*   **Authentication & Security**: Role-based access control (RBAC) securely managed via ASP.NET Identity.

## Getting Started

### Prerequisites

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
*   SQL Server (LocalDB or full instance)

### Installation

1.  Clone the repository.
2.  Update the connection string in `appsettings.json` if necessary.
3.  Apply usage migrations:
    ```bash
    dotnet ef database update
    ```
4.  Run the application:
    ```bash
    dotnet run
    ```

### Database Seeding

The application includes a built-in data seeder that helps initialize the environment with sample data. By default, the application **does not** seed data automatically. You must explicitly run the command with the `--seed` flag.

**To Run with Seeding:**
```bash
dotnet run --seed
```

**How it works:**
*   **Manual Trigger**: The seeder only runs if you provide the `--seed` argument.
*   **Safety Check**: It still checks if users exist to prevent overwriting or duplicating data on an existing database.
*   **Data Created**: Default Roles, Users, Departments, Projects, etc.

**To Reset & Re-seed:**
1.  Delete the existing database.
2.  Run `dotnet run --seed`.

### Database Access (SSMS)

To view your data in SQL Server Management Studio:
*   **Server name**: `(localdb)\mssqllocaldb`
*   **Authentication**: `Windows Authentication`
*   **Database**: `ProjektDB`



## Credentials

The following accounts are available for testing:

*   **Administrator Access**:
    *   **Email**: `admin@nexus.com`
    *   **Password**: `Password123!`
*   **HR Manager**:
    *   **Email**: `hr@nexus.com`
    *   **Password**: `Password123!`
*   **Project Manager**:
    *   **Email**: `manager@nexus.com`
    *   **Password**: `Password123!`
*   **Standard Employee**:
    *   **Email**: `employee@nexus.com`
    *   **Password**: `Password123!`

## Architecture

The project follows the Model-View-Controller (MVC) architectural pattern:

*   **Controllers**: Handle user input and interact with the model.
*   **Models**: Represent the data and business logic (Entity Framework Core entities).
*   **Views**: Render the user interface using Razor syntax.
*   **Data**: `ApplicationDbContext` manages database interactions.
