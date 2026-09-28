using AIEmployeeLeaveManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace AIEmployeeLeaveManagement.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
}
