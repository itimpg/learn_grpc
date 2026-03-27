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
    var greetingResponse = await greetingClient.GreetAsync(greetingRequest);
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

    var longGreetRequest = new LongGreetRequest { Greeting = greeting };
    var longGreetStream = greetingClient.LongGreet();
    foreach (int i in Enumerable.Range(1, 10))
    {
        await longGreetStream.RequestStream.WriteAsync(longGreetRequest);
    }
    await longGreetStream.RequestStream.CompleteAsync();
    var longGreetResponse = await longGreetStream.ResponseAsync;
    Console.WriteLine(longGreetResponse.Result);
    Console.WriteLine("");

}

{
    Console.WriteLine($"====Unary Example====");
    var client = new CalculatorService.CalculatorServiceClient(channel);
    var sumRequest = new SumRequest { A = 10, B = 20 };
    var sumResponse = await client.SumAsync(sumRequest);
    Console.Write($"Sum of {sumRequest.A} and {sumRequest.B} is : ");
    Console.WriteLine(sumResponse.Result);
    Console.WriteLine("");

    Console.WriteLine($"====Server Streaming Example====");
    var pRequest = new PrimeNumberDecompositionRequest { Number = 210 };
    var pResponse = client.PrimeNumberDecomposition(pRequest);
    Console.Write($"Prime number decomposition of {pRequest.Number} is :");
    while (await pResponse.ResponseStream.MoveNext())
    {
        Console.Write(pResponse.ResponseStream.Current.PrimeFactor + ", ");
        await Task.Delay(200);
    }
    Console.WriteLine("");
    Console.WriteLine("");

    Console.WriteLine($"====Client Streaming Example====");
    var avgRequestStream = client.ComputeAverage();
    var inputs = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    foreach (var input in inputs)
    {
        await avgRequestStream.RequestStream.WriteAsync(new ComputeAverageRequest { Number = input });
    }
    await avgRequestStream.RequestStream.CompleteAsync();
    var avgResponse = await avgRequestStream.ResponseAsync;
    Console.Write($"Avg of {string.Join(",", inputs)} is :");
    Console.WriteLine(avgResponse.Average);
    Console.WriteLine("");
}

await channel.ShutdownAsync();
Console.ReadKey();