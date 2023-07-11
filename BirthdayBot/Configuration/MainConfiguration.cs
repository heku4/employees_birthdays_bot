namespace BirthdayBot.Configuration;

public class MainConfiguration
{
    private readonly string _token;
    private readonly int _chatId;

    public MainConfiguration(string token, int chatId)
    {
        _token = token;
        _chatId = chatId;
    }

    public int GetChatId()
    {
        return _chatId;
    }
}