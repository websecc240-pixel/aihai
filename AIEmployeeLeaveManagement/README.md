# AI-Based Employee Leave Management System

A ready-to-run **ASP.NET Core MVC (.NET 8) + C# + Entity Framework Core + SQLite** project based on the supplied requirements.

## Features
- Employee CRUD: create, edit, delete and list employees.
- Leave CRUD: apply, edit, delete and view leave history.
- Validation: date range and leave-balance validation.
- Approval workflow: Pending → Approved/Rejected.
- Automatic leave-balance deduction when a pending request is approved.
- Dashboard with employee/request/pending/approved-day counts.
- AI-style leave pattern analysis and suggestions using a service class; no paid API key required.
- Responsive purple dashboard UI.

## Requirements
- .NET 8 SDK
- Visual Studio 2022 (ASP.NET and web development workload) or VS Code

## Run
```bash
dotnet restore
dotnet run
```
Then open the localhost URL printed by the terminal. The SQLite database `leave.db` is created automatically and seeded with sample data on first run.

## Main classes
- `Models/Employee.cs` – Employee properties and validation
- `Models/LeaveRequest.cs` – Leave properties, status and day calculation
- `Services/LeaveAnalysisService.cs` – pattern analysis/suggestions
- `Data/AppDbContext.cs` – EF Core database context
- `Controllers/EmployeesController.cs` – employee CRUD
- `Controllers/LeavesController.cs` – leave CRUD + approval workflow
