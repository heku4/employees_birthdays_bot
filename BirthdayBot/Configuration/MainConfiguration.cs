namespace BirthdayBot.Configuration;

public class MainConfiguration
{
    private readonly string _token;
    private readonly string _connectionString;
    private readonly long _chatId;

    public MainConfiguration(string token, long chatId, string connectionString)
    {
        _token = token;
        _chatId = chatId;
        _connectionString = connectionString;
    }

    public long GetChatId()
    {
        return _chatId;
    }
    
    public string GetConnectionString()
    {
        return _connectionString;
    }
}