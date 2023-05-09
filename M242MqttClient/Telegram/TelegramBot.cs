using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace M242MqttClient.Telegram;

public class TelegramBot
{
    private readonly Configuration _configuration;
    private readonly TelegramBotClient _botClient;
    private readonly HashSet<long> _subscribedChats = new();

    public TelegramBot(Configuration configuration)
    {
        _configuration = configuration;
        _botClient = new TelegramBotClient(configuration.ApiKey);
    }

    public async Task SendMessageToSubscriber(string message)
    {
        foreach (var chatId in _subscribedChats)
        {
            await _botClient.SendTextMessageAsync(chatId, message);
        }
    }

    public async Task StartAsync()
    {
        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // await _botClient.SetMyCommandsAsync(new List<BotCommand>
        // {
        //     new()
        //     {
        //         Command = "Subscribe",
        //         Description = "Get live updates about the parking spaces"
        //     },
        //     new()
        //     {
        //         Command = "Unsubscribe",
        //         Description = "Stop getting live updates about the parking spaces"
        //     }
        // }, cancellationToken: cts.Token);

        // var me = await _botClient.GetMeAsync(cancellationToken: cts.Token);
        //
        // Console.WriteLine($"Start listening for @{me.Username}");
        // Console.ReadLine();
        //
        // // Send cancellation request to stop bot
        // cts.Cancel();
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        switch (messageText)
        {
            case "/Subscribe" when _subscribedChats.Add(chatId):
                await SendMessage("Nice, you are now subscribed.");
                break;
            case "/Subscribe":
                await SendMessage("Are you so hungry for updates that you want to subscribe twice? Request denied!!!");
                break;
            case "/Unsubscribe" when _subscribedChats.Remove(chatId):
                await SendMessage("Did you get a parking spot? Or why are you unsubscribing?");
                break;
            case "/Unsubscribe":
                await SendMessage("What exactly are you trying tot do? You can't unsubscribe if you are not subscribed.");
                break;
            default:
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You said:\n" + messageText,
                    cancellationToken: cancellationToken);
                break;
        }

        async Task SendMessage(string messageToBeSend)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageToBeSend,
                cancellationToken: cancellationToken);
        }

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    }

    Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}