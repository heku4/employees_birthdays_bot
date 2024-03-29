using BirthdayBot.Core.Models;
using System.Data.SQLite;
using BirthdayBot.Configuration;
using BirthdayBot.Core.Services.DbRepository;
using Telegram.Bot;

namespace BirthdayBot.Core.Services;

public class BirthDayChecker : BackgroundService
{
    private readonly BirthdayService _birthdayService;
    private readonly ITelegramBotClient _botClient; 
    private readonly ILogger<BirthDayChecker> _logger;
    private readonly MainConfiguration _configuration;

    public BirthDayChecker(ILogger<BirthDayChecker> logger, 
        ITelegramBotClient botClient,
        BirthdayService birthdayService,
        MainConfiguration configuration)
    {
        _logger = logger;
        _botClient = botClient;
        _birthdayService = birthdayService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextDayAlarm = now.Date + new TimeSpan(1, 10, 0, 0);
            try
            {
                var birthdays = await _birthdayService.GetClosestBirthdays();
                if (!string.IsNullOrWhiteSpace(birthdays))
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: _configuration.ChatId,
                        text: birthdays,
                        cancellationToken: stoppingToken);
                }
                
                if (now.Hour < 15)
                {
                    var secondAlarmTime = now.Date + new TimeSpan(15, 0, 0);
                    _logger.LogInformation($"Next alarm on: {secondAlarmTime.ToString("u").Replace(" ", "T")}");

                    await Task.Delay(secondAlarmTime - now, stoppingToken);
                }

                _logger.LogInformation($"Next alarm on: {nextDayAlarm.ToString("u").Replace(" ", "T")}");

                await Task.Delay(nextDayAlarm - now, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await Task.Delay(nextDayAlarm - now, stoppingToken);
            }
        }
    }
}