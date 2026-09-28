using AIEmployeeLeaveManagement.Models;
namespace AIEmployeeLeaveManagement.Data;
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Employees.Any()) return;
        var e1 = new Employee { Name="Rahul Sharma", Department="IT", Email="rahul@example.com", LeaveBalance=15 };
        var e2 = new Employee { Name="Priya Patil", Department="HR", Email="priya@example.com", LeaveBalance=18 };
        var e3 = new Employee { Name="Amit Joshi", Department="Finance", Email="amit@example.com", LeaveBalance=20 };
        db.Employees.AddRange(e1,e2,e3); db.SaveChanges();
        db.LeaveRequests.AddRange(
            new LeaveRequest { EmployeeId=e1.Id, LeaveType="Casual", StartDate=DateTime.Today.AddDays(-20), EndDate=DateTime.Today.AddDays(-18), Reason="Personal work", Status=LeaveStatus.Approved },
            new LeaveRequest { EmployeeId=e1.Id, LeaveType="Sick", StartDate=DateTime.Today.AddDays(-5), EndDate=DateTime.Today.AddDays(-4), Reason="Fever", Status=LeaveStatus.Approved },
            new LeaveRequest { EmployeeId=e2.Id, LeaveType="Casual", StartDate=DateTime.Today.AddDays(4), EndDate=DateTime.Today.AddDays(6), Reason="Family function", Status=LeaveStatus.Pending }
        ); db.SaveChanges();
    }
}
