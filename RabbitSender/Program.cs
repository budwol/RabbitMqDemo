using RabbitMQ.Client;
using System.Text;

#region Setup RabbitMQ

ConnectionFactory factory = new()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"), // connection string | ToDo --> json
    ClientProvidedName = "Rabbit Sender App"
};

var cnn = factory.CreateConnection();
var channel = cnn.CreateModel();

const string exchangeName = "DemoExchange";
const string routingKey = "demo-routing-key";
const string queName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queName, false, false, false, null);
channel.QueueBind(queName, exchangeName, routingKey);

#endregion

#region SendMessage

for (var i = 0; i < 100; i++)
{
    var messageBodyBytes = Encoding.UTF8.GetBytes($"Sending Message {i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
}

channel.Close();
cnn.Close();

#endregion