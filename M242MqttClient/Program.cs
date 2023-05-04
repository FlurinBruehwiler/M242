using System.Text.Json;
using M242MqttClient;
using M242MqttClient.Mqtt;
using M242MqttClient.Telegram;

var configString = await File.ReadAllTextAsync("appsettings.json");
var configuration = JsonSerializer.Deserialize<Configuration>(configString);

if (configuration is null)
    return;

var storage = new Storage();

var telegramBot = new TelegramBot(configuration);
var mqttClient = new MqttClient(storage, telegramBot, configuration);

var mqttClientTask = mqttClient.StartAsync();
var telegramBotTask = telegramBot.StartAsync();

await mqttClientTask;
await telegramBotTask;

Console.ReadKey();
