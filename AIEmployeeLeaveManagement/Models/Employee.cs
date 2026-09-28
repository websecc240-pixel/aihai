namespace LeaveManagementSystem.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Department { get; set; } = "";
    public int LeaveBalance { get; set; } = 20;

    public override string ToString() =>
        $"#{Id,-3} {Name,-18} {Department,-14} Balance: {LeaveBalance} days";
}
