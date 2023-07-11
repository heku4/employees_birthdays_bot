using System.Text;
using BirthdayBot.Core.Models;
using BirthdayBot.Core.Services.DbRepositiry;

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

    public async Task AddEmployee(string fullName, int day, int month)
    {
        await _sqliteEmployeesOperations.AddEmployee(fullName, day, month);
    }

    public async Task RemoveEmployee(int id)
    {
        await _sqliteEmployeesOperations.RemoveEmployee(id);
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