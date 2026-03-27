using Greet;
using Grpc.Core;
using System.Text;
using static Greet.GreetingService;

namespace server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = string.Format("Hello, {0} {1}!", request.Greeting.FirstName, request.Greeting.LastName);
            return Task.FromResult(new GreetingResponse { Result = result });
        }

        public override async Task GreetManyTimes(
            GreetingManyTimesRequest request,
            IServerStreamWriter<GreetingManyTimesResponse> responseStream,
            ServerCallContext context)
        {
            Console.WriteLine("The server received the request : ");
            Console.WriteLine(request.ToString());

            foreach (int i in Enumerable.Range(1, 10))
            {
                var result = $"{i + 1}: Hello, {request.Greeting.FirstName} {request.Greeting.LastName}";
                await responseStream.WriteAsync(new GreetingManyTimesResponse { Result = result });
            }
        }

        public override async Task<LongGreetResponse> LongGreet(
            IAsyncStreamReader<LongGreetRequest> requestStream,
            ServerCallContext context)
        {
            StringBuilder sb = new StringBuilder();
            while (await requestStream.MoveNext())
            {
                var g = requestStream.Current.Greeting;

                sb.AppendLine($"Hello {g.FirstName} {g.LastName}");
            }

            return new LongGreetResponse { Result = sb.ToString() };
        }
    }
}
