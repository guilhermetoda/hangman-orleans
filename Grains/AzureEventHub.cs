using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.EventHubs;

namespace Fork
{
    public class EventHub
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://hangman-game.servicebus.windows.net/;SharedAccessKeyName=client;SharedAccessKey=F96bLWkqui/coSD5D95fNSQBtRay7SAOuVBC64O5mNw=;EntityPath=hangman";
        private const string EventHubName = "hangman";

        public static async Task ConnectToEventHub()
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            //await eventHubClient.CloseAsync();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        public static async Task SendMessage(Player player)
        {
            try
            {
                var json = JsonConvert.SerializeObject(player);
                Console.WriteLine($"Sending message: {json}");
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(json)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }

        }

    }
    
}