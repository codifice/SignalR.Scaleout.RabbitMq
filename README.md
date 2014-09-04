SignalR.Scaleout.RabbitMq
=========================

A simple RabbitMQ Backplane for SignalR, install via NuGet (https://www.nuget.org/packages/SignalR.Scaleout.RabbitMq/) using:

```
Install-Package SignalR.Scaleout.RabbitMq 
```

This provider is a simple implementation that doesn't require any RabbitMQ plugins.  It's intended to allow a backend 
process to publish directly to the backplane for distribution via the Hubs.

To run the demo:

* Install RabbitMQ (for development purposes http://chocolatey.org/packages/rabbitmq is fine)
* Run the 'DemoServer' website (http://localhost:xxxx/index.html)
* Run the Publisher backend process
