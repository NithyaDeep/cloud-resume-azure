using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResumeCounter.Functions.Storage;

namespace ResumeCounter.Functions;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    var endpoint = Environment.GetEnvironmentVariable("COSMOS_ACCOUNT_ENDPOINT");
                    if (string.IsNullOrWhiteSpace(endpoint))
                        throw new InvalidOperationException("COSMOS_ACCOUNT_ENDPOINT is missing.");

                    return new CosmosClient(endpoint, new DefaultAzureCredential());
                });

                services.AddSingleton<ICounterStore, ResumeCounter.Functions.Storage.CosmosCounterStore>();
            })
            .Build();

        host.Run();
    }
}
