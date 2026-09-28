namespace LeaveManagementSystem.Models;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = "";
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public DateTime AppliedOn { get; set; } = DateTime.Today;

    // Calculated property (inclusive of both dates)
    public int Days => (EndDate.Date - StartDate.Date).Days + 1;

    public override string ToString() =>
        $"#{Id,-3} {Type,-9} {StartDate:dd-MMM-yyyy} -> {EndDate:dd-MMM-yyyy} ({Days}d) [{Status}] {Reason}";
}
