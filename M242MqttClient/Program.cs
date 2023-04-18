using M242MqttClient;
using M242MqttClient.Mqtt;
using M242MqttClient.Telegram;

var storage = new Storage();

var telegramBot = new TelegramBot();
var mqttClient = new MqttClient(storage, telegramBot);

var mqttClientTask = mqttClient.StartAsync();
var telegramBotTask = telegramBot.StartAsync();

await mqttClientTask;
await telegramBotTask;


