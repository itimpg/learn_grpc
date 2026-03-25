using Calculator;
using Dummy;
using Greet;
using Grpc.Core;

const string target = "127.0.0.1:50051";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

channel.ConnectAsync().ContinueWith(task =>
{
    if (task.IsCompletedSuccessfully)
    {
        Console.WriteLine($"Successfully connected to {target}");
    }
    else
    {
        Console.WriteLine($"Failed to connect to {target}: {task.Exception?.GetBaseException().Message}");
    }
});

//var client = new DummyService.DummyServiceClient(channel);

//var client = new GreetingService.GreetingServiceClient(channel);

//var response = client.Greet(new GreetingRequest
//{
//    Greeting = new Greeting
//    {
//        FirstName = "Chatas",
//        LastName = "Chairin"
//    }
//});

var client = new CalculatorService.CalculatorServiceClient(channel);
var response = client.Sum(new SumRequest { A = 10, B = 20 });
Console.WriteLine(response.Result);

channel.ShutdownAsync().Wait();
Console.ReadKey();