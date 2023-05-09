using InfluxDB.Client;

namespace M242MqttClient;

public class Checker
{
    private readonly Storage _storage;
    
    public Checker(Storage storage)
    {
        _storage = storage;
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            Check();
        });
    }

    private void Check()
    {
        foreach (var space in _storage.ParkingSpaces)
        {
            if (space.CurrentlyOccupied == 0 || space.OccupiedSince is null)
            {
                space.Overdue = false;
                continue;
            }

            if ((DateTime.Now - space.OccupiedSince).Value.Seconds > 5)
            {
                space.Overdue = true;
                continue;
            }

            space.Overdue = false;
        }
    }
}