using AIEmployeeLeaveManagement.Data;
using AIEmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AIEmployeeLeaveManagement.Controllers;
public class EmployeesController : Controller
{
    private readonly AppDbContext _db; public EmployeesController(AppDbContext db)=>_db=db;
    public async Task<IActionResult> Index()=>View(await _db.Employees.OrderBy(x=>x.Name).ToListAsync());
    public IActionResult Create()=>View(new Employee());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(Employee model){if(!ModelState.IsValid)return View(model);_db.Add(model);await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Edit(int id){var e=await _db.Employees.FindAsync(id);return e==null?NotFound():View(e);}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(int id,Employee model){if(id!=model.Id)return NotFound();if(!ModelState.IsValid)return View(model);_db.Update(model);await _db.SaveChangesAsync();return RedirectToAction(nameof(Index));}
    public async Task<IActionResult> Delete(int id){var e=await _db.Employees.FindAsync(id);return e==null?NotFound():View(e);}
    [HttpPost,ActionName("Delete"),ValidateAntiForgeryToken] public async Task<IActionResult> DeleteConfirmed(int id){var e=await _db.Employees.FindAsync(id);if(e!=null){_db.Employees.Remove(e);await _db.SaveChangesAsync();}return RedirectToAction(nameof(Index));}
}
