using BirthdayBot;
using BirthdayBot.Configuration;
using BirthdayBot.Core.Services;
using BirthdayBot.Core.Services.DbRepository;
using Telegram.Bot;
using Telegram.Bot.Services;

var configurationFile = $"etc{Path.DirectorySeparatorChar}config.json";

DotNetEnv.Env.Load();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = new MainConfiguration();
        var configuration = new ConfigurationBuilder().AddJsonFile(configurationFile).AddEnvironmentVariables("BOT_").Build();
        configuration.Bind(config);
        services.AddSingleton(config);
        
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));
        
        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                botConfig.BotToken = config.Token;
                TelegramBotClientOptions options = new(botConfig.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
        services.AddHostedService<BirthDayChecker>();
        services.AddSingleton<BirthdayService>();
        services.AddSingleton<SqliteEmployeesOperations>();
    })
    .Build();

var dbOperationsService = host.Services.GetService<SqliteEmployeesOperations>();
if (!await dbOperationsService!.CheckConnection())
{
    Console.Error.WriteLine("No connection to the database");
    Environment.Exit(1);
}

await host.RunAsync();

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
public class BotConfiguration
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; set; } = string.Empty;
}