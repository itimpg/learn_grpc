using Grpc.Core;
using server;
using System.Runtime.CompilerServices;
using static Grpc.Core.Server;

var RegisterGreetingService = (ServiceDefinitionCollection services) =>
{
    services.Add(Greet.GreetingService.BindService(new GreetingServiceImpl()));
};

var RegisterCalcuatorService = (ServiceDefinitionCollection services) =>
{
    services.Add(Calculator.CalculatorService.BindService(new CalculatorServiceImpl()));
};

const int Port = 50051;

Server? server = null;

try
{ 
    server = new Server
    {
        Ports =
        {
            new ServerPort("localhost", Port, ServerCredentials.Insecure)
        },
        Services = { }
    };
    RegisterGreetingService(server.Services);
    RegisterCalcuatorService(server.Services);
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