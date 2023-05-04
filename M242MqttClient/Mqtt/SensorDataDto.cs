using System.Text.Json.Serialization;

namespace M242MqttClient.Mqtt;

public class SensorDataDto
{
    [JsonPropertyName("isOccupied")]
    public int IsOccupied { get; set; }
    
    [JsonPropertyName("parkSpace")]
    public string ParkSpace { get; set; } = null!;
}