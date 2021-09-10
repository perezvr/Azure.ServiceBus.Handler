using Azure.Messaging.ServiceBus;
using Azure.ServiceBus.Handler.Application.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace Azure.ServiceBus.Handler.Application.Infrastructure.Connector
{
    public class ServiceBusConnector
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceBusLoginCredentials serviceBusLoginCredentials;

        public ServiceBusConnector(IConfiguration configuration)
        {
            _configuration = configuration;

            serviceBusLoginCredentials = new ServiceBusLoginCredentials(
                _configuration.GetSection("ServiceBusLoginCredentials:Namespace").Value ?? null,
                _configuration.GetSection("ServiceBusLoginCredentials:KeyName").Value ?? null,
                _configuration.GetSection("ServiceBusLoginCredentials:Key").Value) ?? null;
        }

        public ServiceBusClient GetServiceBusClient()
        {
            serviceBusLoginCredentials.Validate();
            return new ServiceBusClient(serviceBusLoginCredentials.GetConnectionString());
        }

        public ServiceBusClient GetServiceBusClient(ServiceBusLoginCredentials credentials)
        {
            credentials.Validate();
            return new ServiceBusClient(credentials.GetConnectionString());
        }
    }
}
