using System.Data;
using System.Data.SQLite;
using BirthdayBot.Configuration;
using BirthdayBot.Core.Models;

namespace BirthdayBot.Core.Services.DbRepository;

public class SqliteEmployeesOperations
{
    private readonly ILogger<SqliteEmployeesOperations> _logger;
    private readonly string _connectionString;

    public SqliteEmployeesOperations(ILogger<SqliteEmployeesOperations> logger, MainConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString();
    }

    public async Task<bool> CheckConnection()
    {
        await using var db = new SQLiteConnection($"Data Source={_connectionString}");
        try
        {
            await db.OpenAsync();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM employees LIMIT 1";
            await using var reader = await command.ExecuteReaderAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
        finally
        {
            await db.CloseAsync();
        }
    }

    public async Task<List<Employee>> GetAll()
    {
        await using var db = new SQLiteConnection($"Data Source={_connectionString}");
        try
        {
            await db.OpenAsync();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM employees ORDER BY month, day";
            await using var reader = await command.ExecuteReaderAsync();

            var employees = new List<Employee>();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3))
                    );
                }
            }

            return employees;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new List<Employee>();
        }
        finally
        {
            await db.CloseAsync();
        }
    }

    public async Task<List<Employee>> GetAllOnSpecificMonth(int monthNumber)
    {
        await using var db = new SQLiteConnection($"Data Source={_connectionString}");
        try
        {
            await db.OpenAsync();
            var command = db.CreateCommand();
            command.CommandText = $"SELECT * FROM employees WHERE month = {monthNumber} ORDER BY month, day";
            await using var reader = await command.ExecuteReaderAsync();

            var employees = new List<Employee>();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3))
                    );
                }
            }

            return employees;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new List<Employee>();
        }
        finally
        {
            await db.CloseAsync();
        }
    }
    
    public async Task AddEmployee(string fullName, int dayNumber, int monthNumber, CancellationToken ct)
    {
        await using var db = new SQLiteConnection($"Data Source={_connectionString}");
        try
        {
            await db.OpenAsync(ct);
            var command = db.CreateCommand();
            command.CommandText = $"INSERT INTO employees (name, day, month) VALUES (@fullName, @day, @month) ";
            command.Parameters.AddWithValue("@fullName", fullName).DbType = DbType.String;
            command.Parameters.AddWithValue("@day", dayNumber).DbType = DbType.Int32;
            command.Parameters.AddWithValue("@month", monthNumber).DbType = DbType.Int32;

            var transaction = await db.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            await command.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
        finally
        {
            await db.CloseAsync();
        }
    }
    
    public async Task RemoveEmployee(int id, CancellationToken ct)
    {
        await using var db = new SQLiteConnection($"Data Source={_connectionString}");
        try
        {
            await db.OpenAsync(ct);
            var command = db.CreateCommand();
            command.CommandText = $"DELETE FROM employees WHERE id = @id ";
            command.Parameters.AddWithValue("@id", id).DbType = DbType.Int32;
            
            var transaction = await db.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            await command.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}