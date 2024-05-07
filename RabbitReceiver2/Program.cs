using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

#region Setup RabbitMQ

ConnectionFactory factory = new()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"), // connection string | ToDo --> json
    ClientProvidedName = "Rabbit Receiver1 App"
};

var cnn = factory.CreateConnection();
var channel = cnn.CreateModel();

const string exchangeName = "DemoExchange";
const string routingKey = "demo-routing-key";
const string queName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queName,false, false, false,null);
channel.QueueBind(queName,exchangeName,routingKey);

// RECEIVER
channel.BasicQos(0,1,false);

#endregion

#region Receive Message

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    // simulate business
    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
    
    var body = args.Body.ToArray();
    var msg = Encoding.UTF8.GetString(body);
    
    Console.WriteLine($"Message received: {msg}");
    
    // notify the server that the operation is completed successfully
    channel.BasicAck(args.DeliveryTag,false);
};

var consumerTag = channel.BasicConsume(queName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close();

#endregion