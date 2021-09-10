using Azure.ServiceBus.Handler.Application.ProtobufSerializer;
using Azure.ServiceBus.Handler.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Azure.ServiceBus.Handler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private readonly ServiceBusService _serviceBusService;

        public ServiceBusController(
            ServiceBusService serviceBusService
            )
        {
            _serviceBusService = serviceBusService;
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            await _serviceBusService.WriteAsync("ceresqueuetest", new { Name = "Johm Smith" });
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndExecuteQueueProcessor()
        {
            var serviceResponse = await _serviceBusService.ReadAsync("ceresqueuetest");
            return Ok(Serializer.Deserialize<dynamic>(serviceResponse.Data));
        }
    }
}
