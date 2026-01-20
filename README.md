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



## Credentials

The following accounts are available for testing:

*   **Administrator Access**:
    *   **Email**: `admin@company.com`
    *   **Password**: `Password1234!`
*   **Standard User**:
    *   **Email**: `john.doe@company.com`
    *   **Password**: `Password1234!`

## Architecture

The project follows the Model-View-Controller (MVC) architectural pattern:

*   **Controllers**: Handle user input and interact with the model.
*   **Models**: Represent the data and business logic (Entity Framework Core entities).
*   **Views**: Render the user interface using Razor syntax.
*   **Data**: `ApplicationDbContext` manages database interactions.