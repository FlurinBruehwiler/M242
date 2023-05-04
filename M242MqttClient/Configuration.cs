namespace M242MqttClient;

public class Configuration
{
    public string ClientId { get; set; } = null!;
    public string MqttBroker { get; set; } = null!;
    public string SensorTopic { get; set; } = null!;
    public string PublishTopic { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
}