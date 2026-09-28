using System.Globalization;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Services;

var service = new LeaveService();
var analyzer = new LeaveAnalyzer();

while (true)
{
    Console.WriteLine("""

    ===== AI-Based Employee Leave Management System =====
    1. Add employee          6. Approve / Reject leave
    2. List employees        7. Cancel leave
    3. Apply for leave       8. Delete leave
    4. View leave history    9. Analyze leave patterns (AI suggestions)
    5. Edit pending leave    0. Exit
    """);
    Console.Write("Choose: ");
    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1":
                var e = service.AddEmployee(Ask("Name: "), Ask("Department: "), AskInt("Leave balance (default 20): ", 20));
                Console.WriteLine($"Added: {e}");
                break;
            case "2":
                foreach (var emp in service.GetEmployees()) Console.WriteLine(emp);
                if (service.GetEmployees().Count == 0) Console.WriteLine("No employees yet.");
                break;
            case "3":
                var type = AskEnum<LeaveType>("Type (Casual/Sick/Annual/Emergency): ");
                var l = service.ApplyLeave(AskInt("Employee ID: "), type,
                    AskDate("Start date (yyyy-MM-dd): "), AskDate("End date (yyyy-MM-dd): "), Ask("Reason: "));
                Console.WriteLine($"Applied: {l}");
                break;
            case "4":
                var hist = service.GetHistory(AskInt("Employee ID: "));
                if (hist.Count == 0) Console.WriteLine("No leave records.");
                foreach (var h in hist) Console.WriteLine(h);
                break;
            case "5":
                service.UpdateLeave(AskInt("Leave ID: "), AskDate("New start (yyyy-MM-dd): "),
                    AskDate("New end (yyyy-MM-dd): "), Ask("New reason: "));
                Console.WriteLine("Updated.");
                break;
            case "6":
                int id = AskInt("Leave ID: ");
                service.Decide(id, Ask("Approve? (y/n): ").ToLower() == "y");
                Console.WriteLine($"Result: {service.GetLeave(id)}");
                break;
            case "7":
                service.CancelLeave(AskInt("Leave ID: "));
                Console.WriteLine("Cancelled.");
                break;
            case "8":
                Console.WriteLine(service.DeleteLeave(AskInt("Leave ID: ")) ? "Deleted." : "Not found.");
                break;
            case "9":
                var target = service.GetEmployee(AskInt("Employee ID: "));
                if (target == null) { Console.WriteLine("Employee not found."); break; }
                Console.WriteLine($"\n--- Analysis for {target.Name} ---");
                foreach (var t in analyzer.Analyze(target, service.GetHistory(target.Id))) Console.WriteLine("• " + t);
                Console.WriteLine("\n--- Department overview ---");
                foreach (var d in analyzer.AnalyzeDepartment(service.GetEmployees(), service.GetAllLeaves())) Console.WriteLine("• " + d);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static string Ask(string prompt) { Console.Write(prompt); return Console.ReadLine() ?? ""; }

static int AskInt(string prompt, int? fallback = null)
{
    while (true)
    {
        var s = Ask(prompt);
        if (string.IsNullOrWhiteSpace(s) && fallback.HasValue) return fallback.Value;
        if (int.TryParse(s, out var n)) return n;
        Console.WriteLine("Enter a valid number.");
    }
}

static DateTime AskDate(string prompt)
{
    while (true)
    {
        if (DateTime.TryParseExact(Ask(prompt), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d;
        Console.WriteLine("Use format yyyy-MM-dd.");
    }
}

static T AskEnum<T>(string prompt) where T : struct, Enum
{
    while (true)
    {
        if (Enum.TryParse<T>(Ask(prompt), true, out var v) && Enum.IsDefined(v)) return v;
        Console.WriteLine($"Choose one of: {string.Join(", ", Enum.GetNames<T>())}");
    }
}
