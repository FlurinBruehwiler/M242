using System.Text;
using System.Text.Json;
using M242MqttClient.InfluxDb;
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
    private readonly Configuration _configuration;
    private readonly Influx _influx;
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;

    public MqttClient(Storage storage, TelegramBot telegramBot, Configuration configuration, Influx influx)
    {
        _storage = storage;
        _telegramBot = telegramBot;
        _configuration = configuration;
        _influx = influx;

        _options = new MqttClientOptionsBuilder()
            .WithClientId(configuration.ClientId)
            .WithTcpServer(configuration.MqttBroker)
            .Build();
        
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

        await ProcessMessage(sensorData);
    }

    public async Task ProcessMessage(SensorDataDto sensorData)
    {
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
            if (parkingSpace.CurrentlyOccupied == 1)
            {
                parkingSpace.OccupiedSince = DateTime.Now;
                await _telegramBot.SendMessageToSubscriber("Unlucky you, the left parking space is now occupied. You need to be faster next time!!!");
            }
            else
            {
                parkingSpace.OccupiedSince = null;
                await _telegramBot.SendMessageToSubscriber("The left parking space is free, go take it.");
            }
            
            Console.WriteLine($"{parkingSpace.Key} is now {(parkingSpace.CurrentlyOccupied == 1 ? "occupied" : "free")}");
            
            _influx.SendData(_storage.ParkingSpaces);
            
            var message = _storage.GetOverviewAsJson();

            Console.WriteLine(message);
            
            var applicationMessage = MqttApplicationMessageFactory.Create(new MqttPublishPacket
            {
                Topic = _configuration.PublishTopic,
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
                { new() { Topic = _configuration.SensorTopic, QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce } }
        });

        Console.WriteLine("### CONNECTION ESTABLISHED ###");
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