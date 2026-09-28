using System.Text.Json;
using System.Text.Json.Serialization;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Services;

public class LeaveService
{
    private class Store
    {
        public List<Employee> Employees { get; set; } = new();
        public List<LeaveRequest> Leaves { get; set; } = new();
    }

    private readonly string _file;
    private Store _store = new();
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public LeaveService(string file = "leave_data.json")
    {
        _file = file;
        Load();
    }

    // ---------- Persistence ----------
    private void Load()
    {
        if (!File.Exists(_file)) return;
        try { _store = JsonSerializer.Deserialize<Store>(File.ReadAllText(_file), Options) ?? new(); }
        catch (JsonException) { _store = new(); }
    }

    private void Save() => File.WriteAllText(_file, JsonSerializer.Serialize(_store, Options));

    // ---------- Employee CRUD ----------
    public Employee AddEmployee(string name, string department, int balance = 20)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(department)) throw new ArgumentException("Department is required.");
        if (balance < 0 || balance > 60) throw new ArgumentException("Balance must be between 0 and 60.");

        var emp = new Employee
        {
            Id = _store.Employees.Count == 0 ? 1 : _store.Employees.Max(e => e.Id) + 1,
            Name = name.Trim(),
            Department = department.Trim(),
            LeaveBalance = balance
        };
        _store.Employees.Add(emp);
        Save();
        return emp;
    }

    public IReadOnlyList<Employee> GetEmployees() => _store.Employees;
    public Employee? GetEmployee(int id) => _store.Employees.FirstOrDefault(e => e.Id == id);

    public bool DeleteEmployee(int id)
    {
        var emp = GetEmployee(id);
        if (emp == null) return false;
        _store.Employees.Remove(emp);
        _store.Leaves.RemoveAll(l => l.EmployeeId == id);
        Save();
        return true;
    }

    // ---------- Leave CRUD ----------
    public LeaveRequest ApplyLeave(int employeeId, LeaveType type, DateTime start, DateTime end, string reason)
    {
        var emp = GetEmployee(employeeId) ?? throw new ArgumentException("Employee not found.");
        Validate(emp, start, end, reason, null);

        var leave = new LeaveRequest
        {
            Id = _store.Leaves.Count == 0 ? 1 : _store.Leaves.Max(l => l.Id) + 1,
            EmployeeId = employeeId,
            Type = type,
            StartDate = start.Date,
            EndDate = end.Date,
            Reason = reason.Trim()
        };
        _store.Leaves.Add(leave);
        Save();
        return leave;
    }

    public IReadOnlyList<LeaveRequest> GetHistory(int employeeId) =>
        _store.Leaves.Where(l => l.EmployeeId == employeeId)
                     .OrderByDescending(l => l.StartDate).ToList();

    public IReadOnlyList<LeaveRequest> GetAllLeaves() => _store.Leaves;
    public LeaveRequest? GetLeave(int id) => _store.Leaves.FirstOrDefault(l => l.Id == id);

    public void UpdateLeave(int leaveId, DateTime start, DateTime end, string reason)
    {
        var leave = GetLeave(leaveId) ?? throw new ArgumentException("Leave not found.");
        if (leave.Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be edited.");
        var emp = GetEmployee(leave.EmployeeId)!;
        Validate(emp, start, end, reason, leaveId);

        leave.StartDate = start.Date;
        leave.EndDate = end.Date;
        leave.Reason = reason.Trim();
        Save();
    }

    public void Decide(int leaveId, bool approve)
    {
        var leave = GetLeave(leaveId) ?? throw new ArgumentException("Leave not found.");
        if (leave.Status != LeaveStatus.Pending)
            throw new InvalidOperationException("Request already processed.");

        if (approve)
        {
            var emp = GetEmployee(leave.EmployeeId)!;
            if (leave.Days > emp.LeaveBalance)
                throw new InvalidOperationException("Insufficient leave balance.");
            emp.LeaveBalance -= leave.Days;
            leave.Status = LeaveStatus.Approved;
        }
        else leave.Status = LeaveStatus.Rejected;
        Save();
    }

    public void CancelLeave(int leaveId)
    {
        var leave = GetLeave(leaveId) ?? throw new ArgumentException("Leave not found.");
        if (leave.Status == LeaveStatus.Approved)
            GetEmployee(leave.EmployeeId)!.LeaveBalance += leave.Days; // refund
        leave.Status = LeaveStatus.Cancelled;
        Save();
    }

    public bool DeleteLeave(int leaveId)
    {
        var leave = GetLeave(leaveId);
        if (leave == null) return false;
        if (leave.Status == LeaveStatus.Approved)
            GetEmployee(leave.EmployeeId)!.LeaveBalance += leave.Days;
        _store.Leaves.Remove(leave);
        Save();
        return true;
    }

    // ---------- Validation ----------
    private void Validate(Employee emp, DateTime start, DateTime end, string reason, int? ignoreId)
    {
        if (start.Date < DateTime.Today.AddYears(-1))
            throw new ArgumentException("Start date is too far in the past.");
        if (end.Date < start.Date)
            throw new ArgumentException("End date cannot be before start date.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < 3)
            throw new ArgumentException("Please provide a reason (min 3 characters).");

        int days = (end.Date - start.Date).Days + 1;
        if (days > 30) throw new ArgumentException("A single request cannot exceed 30 days.");
        if (days > emp.LeaveBalance)
            throw new ArgumentException($"Insufficient balance. Requested {days}, available {emp.LeaveBalance}.");

        bool overlap = _store.Leaves.Any(l =>
            l.EmployeeId == emp.Id && l.Id != ignoreId &&
            (l.Status == LeaveStatus.Pending || l.Status == LeaveStatus.Approved) &&
            start.Date <= l.EndDate && end.Date >= l.StartDate);
        if (overlap) throw new ArgumentException("These dates overlap with an existing leave request.");
    }
}
