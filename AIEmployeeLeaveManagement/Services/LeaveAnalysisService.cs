using AIEmployeeLeaveManagement.Data;
using AIEmployeeLeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AIEmployeeLeaveManagement.Services;
public class LeaveAnalysisService
{
    private readonly AppDbContext _db;
    public LeaveAnalysisService(AppDbContext db) => _db = db;
    public async Task<List<string>> GetInsightsAsync()
    {
        var leaves = await _db.LeaveRequests.Include(x => x.Employee).ToListAsync();
        var insights = new List<string>();
        if (!leaves.Any()) return new() { "Not enough leave data yet. Add a few requests to generate insights." };
        var dept = leaves.GroupBy(x => x.Employee!.Department).OrderByDescending(g => g.Sum(x => x.Days)).First();
        insights.Add($"{dept.Key} has the highest total leave days ({dept.Sum(x => x.Days)} days) in the current records.");
        var frequent = leaves.GroupBy(x => x.Employee!.Name).OrderByDescending(g => g.Count()).First();
        if (frequent.Count() >= 2) insights.Add($"{frequent.Key} has submitted {frequent.Count()} leave requests; consider reviewing the pattern with the employee.");
        var pending = leaves.Count(x => x.Status == LeaveStatus.Pending);
        if (pending > 0) insights.Add($"There are {pending} pending request(s) requiring manager review.");
        var sick = leaves.Where(x => x.LeaveType.Equals("Sick", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Days);
        if (sick > 0) insights.Add($"Sick leave accounts for {sick} day(s) in the available records.");
        insights.Add("Suggestion: review unusual spikes, repeated short leaves, and low leave balances before approving future requests.");
        return insights;
    }
}
