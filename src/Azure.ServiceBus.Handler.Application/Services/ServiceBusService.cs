using Azure.Messaging.ServiceBus;
using Azure.ServiceBus.Handler.Application.Abstractions.Read;
using Azure.ServiceBus.Handler.Application.Infrastructure.Connector.Interfaces;
using Azure.ServiceBus.Handler.Application.ProtobufSerializer;
using Azure.ServiceBus.Handler.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Azure.ServiceBus.Handler.Application.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly IServiceBusConnector _connector;

        public ServiceBusService(
            IServiceBusConnector connector)
        {
            _connector = connector;
        }

        public async Task<ReadResponse> ReadAsync(string queueName)
        {
            var response = new ReadResponse();
            await using var client = _connector.GetServiceBusClient();
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);

            ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

            if (receivedMessage != null && receivedMessage.Body != null)
            {
                response.Data = receivedMessage.Body.ToArray();
                await receiver.CompleteMessageAsync(receivedMessage);
            }

            return response;
        }

        public async Task WriteAsync(string queueName, object payload)
        {
            var message = new ServiceBusMessage(Serializer.Serialize(payload));
            await using var client = _connector.GetServiceBusClient();

            ServiceBusSender sender = client.CreateSender(queueName);

            await sender.SendMessageAsync(message);
        }
    }
}
