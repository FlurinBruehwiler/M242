using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;

namespace M242MqttClient.InfluxDb;

public class Influx : IDisposable
{
    private readonly Configuration _configuration;
    public InfluxDBClient InfluxDbClient { get; }
    
    public Influx(Configuration configuration)
    {
        _configuration = configuration;

        InfluxDbClient = InfluxDBClientFactory.Create(configuration.InfluxDb.Url, configuration.InfluxDb.ApiKey.ToCharArray());
    }

    public void SendData(IEnumerable<ParkingSpace> parkingSpaces)
    {
        var freeSpaces = parkingSpaces.Count(x => x.CurrentlyOccupied == 0);

        var point = Point
            .Measurement("mem")
            .Field("spaces", freeSpaces)
            .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
        
        using var writeApi = InfluxDbClient.GetWriteApi();
        writeApi.WritePoint(_configuration.InfluxDb.Bucket, _configuration.InfluxDb.Org, point);
    }

    public void Dispose()
    {
        InfluxDbClient.Dispose();
    }
}