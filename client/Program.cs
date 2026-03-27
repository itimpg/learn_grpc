using Calculator;
using Greet;
using Grpc.Core;

const string target = "127.0.0.1:50051";

Channel channel = new Channel(target, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith(task =>
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

{
    var greetingClient = new GreetingService.GreetingServiceClient(channel);

    var greeting = new Greeting
    {
        FirstName = "iTim",
        LastName = "Dev"
    };
    var greetingRequest = new GreetingRequest { Greeting = greeting };
    var greetingResponse = greetingClient.Greet(greetingRequest);
    Console.WriteLine(greetingResponse.Result);
    Console.WriteLine("");

    var request = new GreetingManyTimesRequest { Greeting = greeting };
    var response = greetingClient.GreetManyTimes(request);

    Console.WriteLine("Greeting Count:");
    while (await response.ResponseStream.MoveNext())
    {
        Console.WriteLine(response.ResponseStream.Current.Result);
        await Task.Delay(200);
    }
    Console.WriteLine("");
    Console.WriteLine("");
}

{
    var client = new CalculatorService.CalculatorServiceClient(channel);
    var sumRequest = new SumRequest { A = 10, B = 20 };
    var sumResponse = client.Sum(sumRequest);
    Console.Write($"Sum of {sumRequest.A} and {sumRequest.B} is : ");
    Console.WriteLine(sumResponse.Result);
    Console.WriteLine("");

    var pRequset = new PrimeNumberDecompositionRequest { Number = 210 };
    var pResponse = client.PrimeNumberDecomposition(pRequset);
    Console.Write($"Prime number decomposition of {pRequset.Number} is :");
    while (await pResponse.ResponseStream.MoveNext())
    {
        Console.Write(pResponse.ResponseStream.Current.Result + ", ");
        await Task.Delay(200);
    }
}

await channel.ShutdownAsync();
Console.ReadKey();