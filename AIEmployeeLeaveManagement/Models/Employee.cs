using System.ComponentModel.DataAnnotations;

namespace AIEmployeeLeaveManagement.Models;
public class Employee
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Name { get; set; } = "";
    [Required, StringLength(100)] public string Department { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    public int LeaveBalance { get; set; } = 20;
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
