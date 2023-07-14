using BirthdayBot.Configuration;
using BirthdayBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BirthdayBot.Core.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly BirthdayService _birthdayService;
    private readonly MainConfiguration _configuration;
    private Mode _mode;

    public UpdateHandler(ITelegramBotClient botClient,
        ILogger<UpdateHandler> logger, BirthdayService birthdayService, MainConfiguration configuration)
    {
        _botClient = botClient;
        _logger = logger;
        _birthdayService = birthdayService;
        _configuration = configuration;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } messageText)
            return;

        if (message.Text.Split(' ').Any())
        {
            if (messageText.Split(' ')[0].StartsWith('/'))
            {
                _mode = Mode.Default;
            }
        }
        
        if (message?.Chat.Id != _configuration.ChatId)
        {
            _logger.LogInformation($"Message from unknown chat: {message?.Chat.Id }");
            await SendChatId(_botClient, message!, cancellationToken);
            return;
        }

        var action = messageText.Split(' ')[0].ToLower() switch
        {
            "/check" => CheckNearestBirthdays(_botClient, message, cancellationToken),
            "/get_all" => SendAllEmployeesList(_botClient, message, cancellationToken),
            "/get_current" => SendThisMonthEmployeesList(_botClient, message, cancellationToken),
            "/get_next" => SendNextMonthEmployeesList(_botClient, message, cancellationToken),
            "/add" => AddEmployee(_botClient, message, cancellationToken),
            "/del" => RemoveEmployee(_botClient, message, cancellationToken),
            "/start" => SendInlineKeyboard(_botClient, message, cancellationToken), 
            "/throw" => FailingHandler(_botClient, message, cancellationToken),
            "/drop" => DropMode(_botClient, message, cancellationToken),
            "/help" => Usage(_botClient, message, cancellationToken),
            _ => null
           
        };

        if (action is null)
        {
            return;
        }
        
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Проверить ближайшие ДР", "/check"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Показать всех", "/get_all"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Показать ДР в этом месяце", "/get_current"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Показать ДР в следующий месяц", "/get_next"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить сотрудника", "add"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Убрать сотрудника", "remove"),
                    }
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Выберите действие:",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        static async Task<Message> Usage(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            const string usage = "Usage:\n" +
                                 "/start - открыть панель действий\n";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
        static Task<Message> FailingHandler(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            throw new IndexOutOfRangeException();
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
    }

    #region BotMode

    private async Task<Message> HandleMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            if (_mode == Mode.Default)
            {
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Попробуйте /help или /start",
                    cancellationToken: cancellationToken);
            }

            if (_mode == Mode.Add)
            {
                await AddEmployee(botClient, message, cancellationToken);
            }

            if (_mode == Mode.Remove)
            {
                await RemoveEmployee(botClient, message, cancellationToken);
            }
        
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Готово",
                cancellationToken: cancellationToken);
        }
        finally
        {
            _mode = Mode.Default;
        }
        
    }

    private async Task<Message> DropMode(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        _mode = Mode.Default;
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Базовый режим работы",
            cancellationToken: cancellationToken);
    }

    #endregion

    #region Birthdays

    private async Task<Message> CheckNearestBirthdays(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var allBdText = await _birthdayService.GetClosestBirthdays();
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: allBdText,
            cancellationToken: cancellationToken);
    }

    
    private async Task<Message> SendAllEmployeesList(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var allBdText = await _birthdayService.GetBirthdays();
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: allBdText,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> SendThisMonthEmployeesList(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var allBdText = await _birthdayService.GetCurrentMonthBirthdays();
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: allBdText,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> SendNextMonthEmployeesList(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var allBdText = await _birthdayService.GetNextMonthBirthdays();
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: allBdText,
            cancellationToken: cancellationToken);
    }

    private async Task<Message> AddEmployee(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            var messageParts = message.Text!.Split(',');
            var fullName = messageParts[0].Trim();
            var birthDay = int.Parse(messageParts[1].Trim());
            var birthMonth = int.Parse(messageParts[2].Trim());

            if (birthDay < 1 || birthDay > 31)
            {
                throw new ArgumentException($"Check birth day value");
            }
            
            if (birthMonth < 1 || birthMonth > 12)
            {
                throw new ArgumentException($"Check birth month value");
            }
            
            await _birthdayService.AddEmployee(fullName, birthDay, birthMonth, cancellationToken);
        
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "ok",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Ошибка при добавлении пользователя: {e.Message}",
                cancellationToken: cancellationToken);
        }
    }
    
    private async Task<Message> RemoveEmployee(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = int.Parse(message.Text!.Trim());

            await _birthdayService.RemoveEmployee(id, cancellationToken);
        
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "ok",
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Ошибка при удалении пользователя: {e.Message}",
                cancellationToken: cancellationToken);
        }
    }

    #endregion
    

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackText = $"Получено {callbackQuery.Data}";
        
        
        InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Отменить", "/drop"),
                }
            });

        
        if (callbackQuery.Data == "add")
        {
            callbackText = $"Режим создания пользователя {callbackQuery.Data}";
            var text = $"Введите имя сотрудника, день и месяц через запятую, например:{Environment.NewLine}" +
                       "Сём Сёмыч,29,12";
            
            _mode = Mode.Add;
            await _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message!.Chat.Id,
                text: text,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        
        if (callbackQuery.Data == "remove")
        {
            callbackText = $"Режим удаления пользователя {callbackQuery.Data}";
            _mode = Mode.Remove;
            var text = $"Введите идентификатор сотрудника";
            await _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message!.Chat.Id,
                text: text,
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: callbackText,
            cancellationToken: cancellationToken);

        if (callbackQuery.Data!.StartsWith('/'))
        {
            var message = callbackQuery.Message;
            message!.Text = callbackQuery.Data;
            await BotOnMessageReceived(message, cancellationToken);
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task<Message> SendChatId(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"This chat is not allowed. Your chatId: {message.Chat.Id.ToString()}",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
}