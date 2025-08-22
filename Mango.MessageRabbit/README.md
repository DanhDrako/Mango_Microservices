# RabbitMQ Base Consumer

This document explains how to use the common `RabbitMQBaseConsumer` located in `Mango.MessageRabbit` project.

## Overview

The `RabbitMQBaseConsumer` is an abstract base class that supports both message consumption patterns:

1. **Simple Queue Pattern** - Direct queue consumption
2. **Exchange Pattern** - Exchange-based routing with queue binding

## Usage Patterns

### 1. Simple Queue Pattern (EmailAPI style)

For simple queue-based consumption, override the `QueueName` property:

```csharp
public class MySimpleConsumer : RabbitMQBaseConsumer
{
    protected override string? QueueName => "my.queue.name";

    protected override async Task HandleMessageAsync(string body)
    {
        // Handle your message here
        var message = JsonConvert.DeserializeObject<MyMessage>(body);
        await ProcessMessage(message);
    }
}
```

### 2. Exchange Pattern (OrderAPI style)

For exchange-based consumption with routing keys, override the `ExchangeName` and `Queue` properties:

```csharp
public class MyExchangeConsumer : RabbitMQBaseConsumer
{
    protected override string? ExchangeName => "my.exchange";

    protected override KeyValuePair<string, string> Queue => new(
        "routing.key",      // Routing key
        "queue.name"        // Queue name
    );

    protected override async Task HandleMessageAsync(string body)
    {
        // Handle your message here
        var message = JsonConvert.DeserializeObject<MyMessage>(body);
        await ProcessMessage(message);
    }
}
```

## Configuration

The base consumer connects to RabbitMQ with default settings:

- **Host**: localhost
- **Username**: guest
- **Password**: guest

## Connection Management

The base consumer automatically:

- Creates connection and channel on initialization
- Declares queues and exchanges as needed
- Handles message acknowledgment
- Disposes resources properly

## Error Handling

Connection errors are logged to the console. For production applications, consider implementing proper logging.

## Examples in Codebase

- **EmailAPI**: Uses simple queue pattern (`RabbitMQAuthConsumer`, `RabbitMQCartConsumer`)
- **OrderAPI**: Uses exchange pattern (`RabbitMQOrderConsumer`)

## Migration Notes

When migrating from local base consumers:

1. Add project reference to `Mango.MessageRabbit`
2. Change using statements to `Mango.Message.RabbitMQ.Consumer.Base`
3. Update `HandleMessage(string body)` to `HandleMessageAsync(string body)`
4. Remove old local base consumer files
