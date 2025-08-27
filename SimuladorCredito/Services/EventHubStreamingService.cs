using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using SimuladorCredito.DTO.Responses;
using System.Text.Json;

namespace SimuladorCredito.Services
{
    public class EventHubStreamingService
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;

        public EventHubStreamingService(IConfiguration configuration)
        {
            var section = configuration.GetSection("EventHub");
            _connectionString = section.GetValue<string>("ConnectionString");

            var entityPath = _connectionString.Split(';').FirstOrDefault(s => s.StartsWith("EntityPath="));
            _eventHubName = entityPath != null ? entityPath.Replace("EntityPath=", "") : throw new ArgumentException("EntityPath não encontrado na connection string.");
        }

        public async Task EnviarSimulacaoAsync(RespostaSimulacao simulacao)
        {
            await using var producerClient = new EventHubProducerClient(_connectionString, _eventHubName);

            var eventoJson = JsonSerializer.Serialize(simulacao);
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
            eventBatch.TryAdd(new EventData(System.Text.Encoding.UTF8.GetBytes(eventoJson)));

            await producerClient.SendAsync(eventBatch);
        }
    }
}
