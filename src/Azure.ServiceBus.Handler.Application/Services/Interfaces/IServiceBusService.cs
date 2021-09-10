using Azure.ServiceBus.Handler.Application.Abstractions.Read;
using System.Threading.Tasks;

namespace Azure.ServiceBus.Handler.Application.Services.Interfaces
{
    public interface IServiceBusService
    {
        Task<ReadResponse> ReadAsync(string queueName);
        Task WriteAsync(string queueName, object payload);
    }
}
