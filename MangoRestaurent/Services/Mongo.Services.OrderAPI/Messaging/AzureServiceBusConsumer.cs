using Azure.Messaging.ServiceBus;
using Mongo.Services.OrderAPI.Messages;
using Newtonsoft.Json;
using System.Text;

namespace Mongo.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer
    {
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        }
    }
}
