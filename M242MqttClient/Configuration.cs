namespace M242MqttClient;

public class Configuration
{
    public string ClientId { get; set; } = null!;
    public string MqttBroker { get; set; } = null!;
    public string SensorTopic { get; set; } = null!;
    public string PublishTopic { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public InfluxDbConfiguration InfluxDb { get; set; } = null!;
}

public class InfluxDbConfiguration
{
    public string ApiKey { get; set; } = null!;
    public string Bucket { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Org { get; set; } = null!;
}