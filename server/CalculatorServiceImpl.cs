using Calculator;
using Grpc.Core;
using static Calculator.CalculatorService;

namespace server
{
    public class CalculatorServiceImpl : CalculatorServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SumResponse
            {
                Result = request.A + request.B
            });
        }

        public override async Task PrimeNumberDecomposition(
            PrimeNumberDecompositionRequest request,
            IServerStreamWriter<PrimeNumberDecompositionResponse> responseStream,
            ServerCallContext context)
        {
            int k = 2;
            int n = request.Number;

            while (n > 1)
            {
                if (n % k == 0)
                {
                    await responseStream.WriteAsync(new PrimeNumberDecompositionResponse { Result = k });
                    n /= k;
                }
                else
                {
                    k += 1;
                }
            }
        }
    }
}
