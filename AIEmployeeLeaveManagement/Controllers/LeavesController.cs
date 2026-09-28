using AIEmployeeLeaveManagement.Data;
using AIEmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AIEmployeeLeaveManagement.Controllers;
public class LeavesController : Controller
{
    private readonly AppDbContext _db; public LeavesController(AppDbContext db)=>_db=db;
    public async Task<IActionResult> Index()=>View(await _db.LeaveRequests.Include(x=>x.Employee).OrderByDescending(x=>x.AppliedOn).ToListAsync());
    public async Task<IActionResult> Create(){ViewBag.Employees=await _db.Employees.OrderBy(x=>x.Name).ToListAsync();return View(new LeaveRequest{StartDate=DateTime.Today,EndDate=DateTime.Today});}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(LeaveRequest model){if(model.EndDate.Date<model.StartDate.Date)ModelState.AddModelError("EndDate","End date must be on or after start date.");var emp=await _db.Employees.FindAsync(model.EmployeeId);if(emp==null)ModelState.AddModelError("EmployeeId","Select a valid employee.");else if(model.Days>emp.LeaveBalance)ModelState.AddModelError("EndDate","Requested days exceed the employee's leave balance.");if(!ModelState.IsValid){ViewBag.Employees=await _db.Employees.OrderBy(x=>x.Name).ToListAsync();return View(model);}model.Status=LeaveStatus.Pending;model.AppliedOn=DateTime.Now;_db.Add(model);await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Edit(int id){var l=await _db.LeaveRequests.FindAsync(id);if(l==null)return NotFound();ViewBag.Employees=await _db.Employees.OrderBy(x=>x.Name).ToListAsync();return View(l);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,LeaveRequest model){if(id!=model.Id)return NotFound();if(model.EndDate<model.StartDate)ModelState.AddModelError("EndDate","End date must be on or after start date.");if(!ModelState.IsValid){ViewBag.Employees=await _db.Employees.OrderBy(x=>x.Name).ToListAsync();return View(model);} _db.Update(model);await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Approve(int id){var l=await _db.LeaveRequests.Include(x=>x.Employee).FirstOrDefaultAsync(x=>x.Id==id);if(l!=null&&l.Status==LeaveStatus.Pending){if(l.Days<=l.Employee!.LeaveBalance){l.Status=LeaveStatus.Approved;l.Employee.LeaveBalance-=l.Days;await _db.SaveChangesAsync();}}return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Reject(int id){var l=await _db.LeaveRequests.FindAsync(id);if(l!=null&&l.Status==LeaveStatus.Pending){l.Status=LeaveStatus.Rejected;await _db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Delete(int id){var l=await _db.LeaveRequests.Include(x=>x.Employee).FirstOrDefaultAsync(x=>x.Id==id);return l==null?NotFound():View(l);}
    [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id){var l=await _db.LeaveRequests.FindAsync(id);if(l!=null){_db.Remove(l);await _db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}
