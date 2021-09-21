using Azure.Messaging.ServiceBus;
using Azure.ServiceBus.Handler.Application.ProtobufSerializer;
using Azure.ServiceBus.Handler.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        public async Task<ActionResult> Get()
        {
            var serviceResponse = await _serviceBusService.ReadAsync("ceresqueuetest");
            return Ok(Serializer.Deserialize<dynamic>(serviceResponse.Data));
        }

        [HttpGet("processor")]
        public async Task<ActionResult> CreateAndExecuteQueueProcessor()
        {
            string connectionString = "Endpoint=sb://ceresnonproddotnetservice.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Kxu/g5LYV+CbFo+SqR+ZUxxq5MKEsQDFwRwVSYFeFZo=";
            string queueName = "ceresqueuetest";

            await using var client = new ServiceBusClient(connectionString);

            // create the options to use for configuring the processor
            var options = new ServiceBusProcessorOptions
            {
                // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                AutoCompleteMessages = false,

                // I can also allow for multi-threading
                MaxConcurrentCalls = 2
            };

            // create a processor that we can use to process the messages
            await using ServiceBusProcessor processor = client.CreateProcessor(queueName, options);

            // configure the message and error handler to use
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            async Task MessageHandler(ProcessMessageEventArgs args)
            {
                var body = args.Message.Body.ToArray();
                Console.WriteLine(body);

                var obj = Serializer.Deserialize<dynamic>(body);

                // we can evaluate application logic and use that to determine how to settle the message.
                await args.CompleteMessageAsync(args.Message);
            }

            Task ErrorHandler(ProcessErrorEventArgs args)
            {
                // the error source tells me at what point in the processing an error occurred
                Console.WriteLine(args.ErrorSource);
                // the fully qualified namespace is available
                Console.WriteLine(args.FullyQualifiedNamespace);
                // as well as the entity path
                Console.WriteLine(args.EntityPath);
                Console.WriteLine(args.Exception.ToString());

                string path = @"c:\files\log\log.txt";
                // This text is added only once to the file.
                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine($"{DateTime.Now,19}|{args.Exception.Message,80}");
                    }
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(path))
                    {
                        sw.WriteLine($"{DateTime.Now,30}|{args.Exception.Message,80}");
                    }
                }
                // This text is always added, making the file longer over time
                // if it is not deleted.

                return Task.CompletedTask;
            }

            // start processing
            await processor.StartProcessingAsync();

            //// since the processing happens in the background, we add a Conole.ReadKey to allow the processing to continue until a key is pressed.
            Console.ReadKey();


            return Ok();
        }
    }
}
