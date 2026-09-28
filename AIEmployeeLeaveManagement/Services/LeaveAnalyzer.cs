using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Services;

/// <summary>Rule-based "AI" that analyzes leave patterns and gives suggestions.</summary>
public class LeaveAnalyzer
{
    public List<string> Analyze(Employee emp, IReadOnlyList<LeaveRequest> history)
    {
        var tips = new List<string>();
        var valid = history.Where(l => l.Status != LeaveStatus.Cancelled).ToList();

        if (valid.Count == 0)
        {
            tips.Add("No leave history yet. Remember to take regular breaks to avoid burnout.");
            return tips;
        }

        var approved = valid.Where(l => l.Status == LeaveStatus.Approved).ToList();
        int used = approved.Sum(l => l.Days);
        tips.Add($"Summary: {valid.Count} requests, {used} approved days used, {emp.LeaveBalance} days left.");

        int edge = valid.Count(l => l.StartDate.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Friday);
        if (valid.Count >= 3 && edge * 2 >= valid.Count)
            tips.Add("Pattern: most leaves start on a Monday or Friday (long-weekend habit). Consider planning coverage in advance.");

        int sick = valid.Count(l => l.Type == LeaveType.Sick);
        if (sick >= 3)
            tips.Add($"Pattern: {sick} sick leaves recorded. A health check-up or workload review may help.");

        int lastMinute = valid.Count(l => (l.StartDate - l.AppliedOn).Days <= 1);
        if (lastMinute >= 2)
            tips.Add($"Pattern: {lastMinute} requests were made with 1 day or less notice. Apply earlier when possible.");

        int rejected = valid.Count(l => l.Status == LeaveStatus.Rejected);
        if (valid.Count >= 3 && rejected * 100 / valid.Count >= 40)
            tips.Add("Pattern: many requests were rejected. Try avoiding peak periods and add clearer reasons.");

        if (emp.LeaveBalance <= 3)
            tips.Add("Low balance: keep the remaining days for emergencies.");
        else if (emp.LeaveBalance >= 15 && used < 5)
            tips.Add("Balance is high and little leave was taken. Consider scheduling a break to avoid burnout.");

        var top = valid.GroupBy(l => l.Type).OrderByDescending(g => g.Count()).First();
        tips.Add($"Most frequent leave type: {top.Key} ({top.Count()} times).");

        return tips;
    }

    public List<string> AnalyzeDepartment(IReadOnlyList<Employee> employees, IReadOnlyList<LeaveRequest> all)
    {
        var lines = new List<string>();
        foreach (var g in employees.GroupBy(e => e.Department))
        {
            var ids = g.Select(e => e.Id).ToHashSet();
            int days = all.Where(l => ids.Contains(l.EmployeeId) && l.Status == LeaveStatus.Approved).Sum(l => l.Days);
            lines.Add($"{g.Key}: {g.Count()} employees, {days} approved leave days ({(double)days / g.Count():0.0} per person)");
        }
        return lines;
    }
}
