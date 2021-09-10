using Azure.Messaging.ServiceBus;
using Azure.ServiceBus.Handler.Application.Infrastructure.Configuration;

namespace Azure.ServiceBus.Handler.Application.Infrastructure.Connector.Interfaces
{
    public interface IServiceBusConnector
    {
        ServiceBusClient GetServiceBusClient();
        ServiceBusClient GetServiceBusClient(ServiceBusLoginCredentials credentials);
    }
}
