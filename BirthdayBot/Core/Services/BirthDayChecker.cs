using BirthdayBot.Core.Models;
using System.Data.SQLite;
using BirthdayBot.Core.Services.DbRepository;
using Telegram.Bot;

namespace BirthdayBot.Core.Services;

public class BirthDayChecker : BackgroundService
{
    private readonly BirthdayService _birthdayService;
    private readonly ITelegramBotClient _botClient; 
    private readonly ILogger<BirthDayChecker> _logger;

    public BirthDayChecker(ILogger<BirthDayChecker> logger, ITelegramBotClient botClient, BirthdayService birthdayService)
    {
        _logger = logger;
        _botClient = botClient;
        _birthdayService = birthdayService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var birthdays = await _birthdayService.GetClosestBirthdays();
                if (!string.IsNullOrWhiteSpace(birthdays))
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: 793853813,
                        text: birthdays,
                        cancellationToken: stoppingToken);
                }
                
                var now = DateTime.Now;
                if (now.Hour < 15)
                {
                    var secondAlarmTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);
                    _logger.LogInformation(secondAlarmTime.ToString("u").Replace(" ", "T"));
                    await Task.Delay(secondAlarmTime - now, stoppingToken);
                }
                
                var nextDayAlarm = new DateTime(now.Year, now.Month, now.Day + 1, 10, 0, 0);
                _logger.LogInformation(nextDayAlarm.ToString("u").Replace(" ", "T"));
                await Task.Delay(nextDayAlarm - now, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}