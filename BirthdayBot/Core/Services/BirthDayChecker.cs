using BirthdayBot.Core.Models;
using System.Data.SQLite;
using BirthdayBot.Core.Services.DbRepository;

namespace BirthdayBot.Core.Services;

public class BirthDayChecker : BackgroundService
{
    private readonly SqliteEmployeesOperations _sqliteEmployeesOperations;

    private readonly ILogger<BirthDayChecker> _logger;

    //private readonly 
    public BirthDayChecker(ILogger<BirthDayChecker> logger, SqliteEmployeesOperations sqliteEmployeesOperations)
    {
        _logger = logger;
        _sqliteEmployeesOperations = sqliteEmployeesOperations;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await _sqliteEmployeesOperations.CheckConnection())
        {
            _logger.LogCritical("Connection to the DB failed");
            Environment.Exit(1);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}