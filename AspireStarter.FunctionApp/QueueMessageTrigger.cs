using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AspireStarter.FunctionApp
{
    public class QueueMessageTrigger
    {
        private readonly ILogger<QueueMessageTrigger> logger;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public QueueMessageTrigger(ILogger<QueueMessageTrigger> logger, JsonSerializerOptions jsonSerializerOptions)
        {
            this.logger = logger;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function(nameof(QueueMessageTrigger))]
        public void Run([QueueTrigger("messages", Connection = "QueueConnectionString")] CorrelatedMessage message)
        {
            logger.LogInformation($"Message Received: {message.Message} (CorrelationId: {message.CorrelationId})");
        }

        public record CorrelatedMessage(string CorrelationId, string Message);
    }
}
