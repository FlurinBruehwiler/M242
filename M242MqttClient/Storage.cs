using System.Text.Json;

namespace M242MqttClient;

public class Storage
{
    public List<ParkingSpace> ParkingSpaces { get; } = new();

    public string GetOverviewAsJson()
    {
        var left = ParkingSpaces.FirstOrDefault(x => x.Key == "left")?.CurrentlyOccupied ?? 0;
        var right = ParkingSpaces.FirstOrDefault(x => x.Key == "right")?.CurrentlyOccupied ?? 0;
        
        return $"{left}:{right}";
    }
}

public class BackPayload
{
    public int Left { get; set; }
    public int Right { get; set; }
}

public class ParkingSpace
{
    public required string Key { get; set; }
    public int CurrentlyOccupied { get; set; }
    public DateTime? OccupiedSince { get; set; }
    public bool Overdue { get; set; }
}