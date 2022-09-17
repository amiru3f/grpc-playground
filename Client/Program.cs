namespace Client;

using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("going to run grpc-client");
        string grpcAddress = Environment.GetEnvironmentVariable("Api_ADDRESS") ?? "";
        System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(grpcAddress), "Empty grpc address");

        Console.WriteLine(grpcAddress);

        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3000,
                InitialBackoff = TimeSpan.FromSeconds(10),
                MaxBackoff = TimeSpan.FromSeconds(3600),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };

        GrpcChannel channel = GrpcChannel.ForAddress(grpcAddress, new GrpcChannelOptions()
        {
            ServiceConfig = new ServiceConfig()
            {
                MethodConfigs =
                {
                    defaultMethodConfig
                }
            }
        });


        ProductContracts.ProductService.ProductServiceClient client = new ProductContracts.ProductService.ProductServiceClient(channel);

        Console.WriteLine(await client.HelloAsync(new ProductContracts.HelloRequest() { Name = "TestClient" }));

        var streamingResponse = client.StreamReply(new Google.Protobuf.WellKnownTypes.Empty(), new Grpc.Core.Metadata() { }, DateTime.UtcNow.AddDays(1), default);
        while (true)
        {
            try
            {
                if (await streamingResponse.ResponseStream.MoveNext(CancellationToken.None))
                {
                    Console.WriteLine("From stream resposne: " + streamingResponse.ResponseStream.Current);
                }
            }
            catch (Grpc.Core.RpcException)
            {
                await Task.Delay(1000);
                streamingResponse = client.StreamReply(new Google.Protobuf.WellKnownTypes.Empty(), new Grpc.Core.Metadata() { }, DateTime.UtcNow.AddDays(1), default);
            }
        }
    }
}
