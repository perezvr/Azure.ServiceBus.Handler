using System;

namespace Azure.ServiceBus.Handler.Application.Infrastructure.Configuration
{
    public class ServiceBusLoginCredentials
    {
        public ServiceBusLoginCredentials(string sbNamespace, string sbKeyName, string sbKey)
        {
            Namespace = sbNamespace;
            KeyName = sbKeyName;
            Key = sbKey;
        }

        public string Namespace { get; set; }
        public string KeyName { get; set; }
        public string Key { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Namespace))
                throw new Exception("Cannot resolve Service Bus Namespace.");

            if (string.IsNullOrEmpty(KeyName))
                throw new Exception("Cannot resolve Service Bus Keyname.");

            if (string.IsNullOrEmpty(Key))
                throw new Exception("Cannot resolve Service Bus Key.");
        }

        internal string GetConnectionString()
          => string.Format("Endpoint=sb://{0};SharedAccessKeyName={1};SharedAccessKey={2};",
              Namespace, KeyName, Key);
    }
}
