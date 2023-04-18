# M242
MQTT 4 the win

## MQTT Client
Wir verwendet das Package MQTTnet um einen MQTT Client zu erstellen. 
Kommuniziert wird über zwei topics:  

### SensorTopic
Über dieses Topic senden die Sensoren ihren Status, wenn sich dieser geändert hat.
Das Format sieht folgendermassen aus:

```json
{
  "isOccupied": true,
  "parkSpace": "xyz"
}
```

### PublishTopic
Auf diesem Topic sendet dieser Client eine Übersicht des Status aller Parkplätze.
Das Format sieht folgendermassen aus:
```json
{
  "xyz": true,
  "abc": false
}
```

## Telegram Bot
Man kann beim Telegram Bot subscriben, dann bekommt man immer eine Nachricht wenn sich der Status von einem der Parkplätze ändert.

Es gibt zwei Commands
/Subscribe:
/Unsubscribe:


## Configuration
Create a M242MqttClient/appsetting.json file with the following content:

```json
{
    "General": {
        "ClientId": "MyKoohlClient",
        "MqttBroker": "cloud.tbz.ch",
        "SensorTopic": "garagepp/core",
        "PublishTopic": "garagepp/eroc",
        "ApiKey": "YOURAPIKEY"
    }
}
```
