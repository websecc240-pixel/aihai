using AIEmployeeLeaveManagement.Data;
using AIEmployeeLeaveManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIEmployeeLeaveManagement.Controllers;
public class HomeController : Controller
{
    private readonly AppDbContext _db; private readonly LeaveAnalysisService _ai;
    public HomeController(AppDbContext db, LeaveAnalysisService ai) { _db=db; _ai=ai; }
    public async Task<IActionResult> Index()
    {
        ViewBag.Employees = await _db.Employees.CountAsync();
        ViewBag.Leaves = await _db.LeaveRequests.CountAsync();
        ViewBag.Pending = await _db.LeaveRequests.CountAsync(x => x.Status == Models.LeaveStatus.Pending);
        ViewBag.ApprovedDays = await _db.LeaveRequests.Where(x=>x.Status==Models.LeaveStatus.Approved).Select(x=>(int?)x.Days).SumAsync() ?? 0;
        ViewBag.Insights = await _ai.GetInsightsAsync();
        return View();
    }
}
