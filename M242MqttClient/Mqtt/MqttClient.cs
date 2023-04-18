using System.Text;
using System.Text.Json;
using M242MqttClient.Telegram;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace M242MqttClient.Mqtt;

public class MqttClient
{
    private readonly Storage _storage;
    private readonly TelegramBot _telegramBot;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;

    public MqttClient(Storage storage, TelegramBot telegramBot)
    {
        _storage = storage;
        _telegramBot = telegramBot;

        var builder = new MqttClientOptionsBuilder()
            .WithClientId("MyKoohlClient")
            .WithTcpServer("cloud.tbz.ch")
            // .WithCredentials("bud", "%spencer%")
            .WithTls()
            .WithCleanSession();

        _options = builder.Build();

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        _client.DisconnectedAsync += OnDisconnectedAsync;
        _client.ConnectedAsync += OnConnectedAsync;
    }

    public async Task StartAsync()
    {
        try
        {
            await _client.ConnectAsync(_options);
        }
        catch
        {
            Console.WriteLine("### CONNECTING FAILED ###");
        }
    }

    private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payloadString = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        var sensorData = JsonSerializer.Deserialize<SensorDataDto>(payloadString);

        if (sensorData is null)
        {
            Console.WriteLine("Sensor Data was null");
            return;
        }

        var parkingSpace = _storage.ParkingSpaces.FirstOrDefault(x => x.Key == sensorData.ParkSpace);

        if (parkingSpace is null)
        {
            parkingSpace = new ParkingSpace
            {
                Key = sensorData.ParkSpace
            };
            _storage.ParkingSpaces.Add(parkingSpace);
        }

        if (parkingSpace.CurrentlyOccupied != sensorData.IsOccupied)
        {
            parkingSpace.CurrentlyOccupied = sensorData.IsOccupied;
            if (parkingSpace.CurrentlyOccupied)
            {
                parkingSpace.OccupiedSince = DateTime.Now;
                await _telegramBot.SendMessageToSubscriber("Unlucky you, the left parking space is now occupied. You need to be faster next time!!!");
            }
            else
            {
                parkingSpace.OccupiedSince = null;
                await _telegramBot.SendMessageToSubscriber("The left parking space is free, go take it.");
            }

            var message = _storage.GetOverviewAsJson();
            
            var applicationMessage = MqttApplicationMessageFactory.Create(new MqttPublishPacket
            {
                Topic = "eroc",
                Payload = Encoding.ASCII.GetBytes(message),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
            });
            await _client.PublishAsync(applicationMessage);
        }
    }

    async Task OnConnectedAsync(MqttClientConnectedEventArgs _)
    {
        await _client.SubscribeAsync(new MqttClientSubscribeOptions
        {
            TopicFilters = new List<MqttTopicFilter>
                { new() { Topic = "core", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce } }
        });
    }

    async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
    {
        await Task.Delay(TimeSpan.FromSeconds(5));

        try
        {
            await _client.ConnectAsync(_options);
        }
        catch
        {
            Console.WriteLine("### RECONNECTING FAILED ###");
        }
    }
}