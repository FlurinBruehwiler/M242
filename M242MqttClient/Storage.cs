using System.Text.Json;

namespace M242MqttClient;

public class Storage
{
    public List<ParkingSpace> ParkingSpaces { get; } = new();

    public string GetOverviewAsJson()
    {
        var dict = ParkingSpaces.ToDictionary(space => space.Key, space => space.CurrentlyOccupied);

        return JsonSerializer.Serialize(dict);
    }
}

public class ParkingSpace
{
    public required string Key { get; set; }
    public bool CurrentlyOccupied { get; set; }
    public DateTime? OccupiedSince { get; set; }
}