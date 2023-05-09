# M242 Miniprojekt - Parkplatzzählung

In diesem Repository befindet sich das Backend der Parkplatzzählung, der Code für die M5Stack2 
befindet sich in [M5Stack-parkingsystem](https://github.com/Z-100/M5Stack-parkingsystem).

Die Testfälle, welche 100% des ganzen Systems testen sind ebenfalls im 
[M5Stack-parkingsystem](https://github.com/Z-100/M5Stack-parkingsystem) aufzufinden. 

## MQTT Client
Wir verwendet das Package MQTTnet um einen MQTT Client zu erstellen. 
Kommuniziert wird über zwei topics:  

### SensorTopic
Über dieses Topic senden die Sensoren ihren Status, wenn sich dieser geändert hat.
Das Format sieht folgendermassen aus:

0: fale
1: true

```json
{
  "isOccupied": 1,
  "parkSpace": "xyz"
}
```

### PublishTopic
Auf diesem Topic sendet dieser Client eine Übersicht des Status aller Parkplätze.
Das Format sieht folgendermassen aus:
```text
0:1
```

## Telegram Bot
Man kann beim Telegram Bot subscriben, dann bekommt man immer eine Nachricht wenn sich der Status von einem der Parkplätze ändert.

Es gibt zwei Commands
/Subscribe:
/Unsubscribe:


## Anleitung "Backend"
Um das Projekt laufen zu lassen muss zuerst die [dotnet 7 SDK](https://dotnet.microsoft.com/en-us/download) installiert werden. 
Danach muss man das Projekt konfigurieren:

Dafür muss die Datei M242MqttClient/appsetting.json mit dem folgenden Inhalt erstellt werden und angepasst werden:

```json
{
    "ClientId": "MyKoohlClient",
    "MqttBroker": "cloud.tbz.ch",
    "SensorTopic": "garagepp/core",
    "PublishTopic": "garagepp/eroc",
    "ApiKey": "YOURAPIKEY"
}
```

Das Projekt kann mit dem folgenden Command laufen gelassen werden
```
cd M242MqttClient
dotnet run
```

## Anleitung "M5Stack Core2"
1. Clone Repository
2. Open it in VS Code
3. Install the PlatformIO Extension
4. Fill in the WiFi Inforrmawtion in requestclient.cpp:10
5. Upload the Code to the M5StackCore2.
	- One should have the boolean isTopM5Stack == true and the other one false in application_state.cpp:14

