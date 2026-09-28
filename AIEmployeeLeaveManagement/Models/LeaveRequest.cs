using System.ComponentModel.DataAnnotations;

namespace AIEmployeeLeaveManagement.Models;
public enum LeaveStatus { Pending, Approved, Rejected }
public class LeaveRequest
{
    public int Id { get; set; }
    [Required] public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    [Required, StringLength(30)] public string LeaveType { get; set; } = "Casual";
    [Required, DataType(DataType.Date)] public DateTime StartDate { get; set; }
    [Required, DataType(DataType.Date)] public DateTime EndDate { get; set; }
    [StringLength(500)] public string? Reason { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime AppliedOn { get; set; } = DateTime.Now;
    public int Days => EndDate.Date < StartDate.Date ? 0 : (EndDate.Date - StartDate.Date).Days + 1;
}
