using System.Text;
using BirthdayBot.Core.Models;
using BirthdayBot.Core.Services.DbRepository;

namespace BirthdayBot.Core.Services;

public class BirthdayService
{
    private readonly SqliteEmployeesOperations _sqliteEmployeesOperations;
    private readonly ILogger<BirthDayChecker> _logger;

    public BirthdayService(ILogger<BirthDayChecker> logger, SqliteEmployeesOperations sqliteEmployeesOperations)
    {
        _logger = logger;
        _sqliteEmployeesOperations = sqliteEmployeesOperations;
    }

    public async Task AddEmployee(string fullName, int day, int month, CancellationToken ct)
    {
        await _sqliteEmployeesOperations.AddEmployee(fullName, day, month, ct);
    }

    public async Task RemoveEmployee(int id, CancellationToken ct)
    {
        await _sqliteEmployeesOperations.RemoveEmployee(id, ct);
    }
    
    public async Task<string> GetClosestBirthdays()
    {
        var employees = await _sqliteEmployeesOperations.GetAll();
        var fiveDaysGuys = employees.Where(e => e.DaysBeforeBirthDate == 5).ToList();
        var oneDayGuys = employees.Where(e => e.DaysBeforeBirthDate == 1).ToList();
        var zeroDayGuys = employees.Where(e => e.DaysBeforeBirthDate == 0).ToList();

        var sb = new StringBuilder();
        if (fiveDaysGuys.Any())
        {
            sb.Append($"Через 5 дней День рождения у:{Environment.NewLine}{FormatEmployeesList(fiveDaysGuys)}{Environment.NewLine}");
        }

        if (oneDayGuys.Any())
        {
            sb.Append($"Завтра День рождения у:{Environment.NewLine}{FormatEmployeesList(oneDayGuys)}{Environment.NewLine}");
        }

        if (zeroDayGuys.Any())
        {
            sb.Append($"Сегодня День рождения у:{Environment.NewLine}{FormatEmployeesList(zeroDayGuys)}{Environment.NewLine}");
        }

        var result = sb.ToString();

        if (string.IsNullOrWhiteSpace(result))
        {
            result = "No employees";
        }
        
        return result;
    }
    
    public async Task<string> GetBirthdays()
    {
        var employees = await _sqliteEmployeesOperations.GetAll();

        return FormatEmployeesList(employees);
    }

    public async Task<string> GetCurrentMonthBirthdays()
    {
        var employees = await _sqliteEmployeesOperations.GetAllOnSpecificMonth(DateTime.Now.Month);

        return FormatEmployeesList(employees);
    }

    public async Task<string> GetNextMonthBirthdays()
    {
        var nextMonth = DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1;
        var employees = await _sqliteEmployeesOperations.GetAllOnSpecificMonth(nextMonth);

        return FormatEmployeesList(employees);
    }

    private string FormatEmployeesList(List<Employee> employees)
    {
        var sb = new StringBuilder();

        if (!employees.Any())
        {
            return "No Birthdays!";
        }
        
        foreach (var employee in employees)
        {
            sb.Append(employee.ToString() + Environment.NewLine);
        }

        return sb.ToString();
    }
}