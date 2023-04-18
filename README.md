# M242
MQTT 4 the win

# Beschreibung
Wir verwendet das Package MQTTnet um einen MQTT Client zu erstellen. 
Kommuniziert wird über zwei topics:  
## SensorTopic
Über dieses Topic senden die Sensoren ihren Status, wenn sich dieser geändert hat.
Das Format sieht folgendermassen aus:

```json
{
  "isOccupied": true,
  "parkSpace": "xyz"
}
```

## PublishTopic
Auf diesem Topic sendet dieser Client eine Übersicht des Status aller Parkplätze.
Das Format sieht folgendermassen aus:
{
  "xyz": true,
  "abc": false
}

# Configuration
Create a M242MqttClient/appsetting.json file with the following content:

```json
{
    "General": {
        "ClientId": "MyKoohlClient",
        "MqttBroker": "cloud.tbz.ch",
        "SensorTopic": "core",
        "PublishTopic": "eroc",
        "ApiKey": "YOURAPIKEY"
    }
}
```
