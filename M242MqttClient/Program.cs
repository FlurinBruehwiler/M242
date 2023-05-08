using System.Text.Json;
using M242MqttClient;
using M242MqttClient.InfluxDb;
using M242MqttClient.Mqtt;
using M242MqttClient.Telegram;

var configString = await File.ReadAllTextAsync("appsettings.json");
var configuration = JsonSerializer.Deserialize<Configuration>(configString);

if (configuration is null)
    return;

var storage = new Storage();

var influx = new Influx(configuration);
var telegramBot = new TelegramBot(configuration);
var mqttClient = new MqttClient(storage, telegramBot, configuration, influx);

var mqttClientTask = mqttClient.StartAsync();
var telegramBotTask = telegramBot.StartAsync();

await mqttClientTask;
await telegramBotTask;

while (true)
{
    Console.ReadLine();

    var rand = new Random();
    var isOccupied = rand.Next(2);
    var parkingSpace = rand.Next(2) == 0 ? "left" : "right";
    
    var data = new SensorDataDto
    {
        IsOccupied = isOccupied,
        ParkSpace = parkingSpace
    };
    
    Console.WriteLine(JsonSerializer.Serialize(data));

    await mqttClient.ProcessMessage(data);
}

