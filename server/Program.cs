using Grpc.Core;
using server;

const int Port = 50051;

Server server = null;

try
{
    server = new Server
    {
        Ports =
        {
            new ServerPort("localhost", Port, ServerCredentials.Insecure)
        },
        Services =
        {
            Greet.GreetingService.BindService(new GreetingServiceImpl()),
            Calculator.CalculatorService.BindService(new CalculatorServiceImpl()),
        }
    };
    server.Start();
    Console.WriteLine($"The server is listening on the port : {Port}");
    Console.ReadKey();
}
catch (IOException ex)
{
    Console.WriteLine($"The server failed to start : {ex.Message}");
}
finally
{
    if (server != null)
    {
        await server.ShutdownAsync();
        Console.WriteLine("Server shutdown completed.");
    }
}