using Microsoft.Azure.Cosmos;
using ResumeCounter.Functions.Models;

namespace ResumeCounter.Functions.Storage;

public class CosmosCounterStore : ICounterStore
{
    private readonly Container _container;

    public CosmosCounterStore(CosmosClient client)
    {
        var dbName = Environment.GetEnvironmentVariable("COSMOS_DB_NAME") ?? "resume";
        var containerName = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_NAME") ?? "counters";
        _container = client.GetContainer(dbName, containerName);
    }

    public async Task<int> GetAsync(CancellationToken ct)
    {
        var item = await ReadOrCreateAsync(ct);
        return item.count;
    }

    public async Task<int> IncrementAsync(CancellationToken ct)
    {
        var item = await ReadOrCreateAsync(ct);
        item.count += 1;

        await _container.UpsertItemAsync(item, new PartitionKey(item.id), cancellationToken: ct);
        return item.count;
    }

    private async Task<CounterDoc> ReadOrCreateAsync(CancellationToken ct)
    {
        try
        {
            var response = await _container.ReadItemAsync<CounterDoc>(
                "visitors",
                new PartitionKey("visitors"),
                cancellationToken: ct);

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var doc = new CounterDoc { id = "visitors", count = 0 };
            var created = await _container.CreateItemAsync(doc, new PartitionKey(doc.id), cancellationToken: ct);
            return created.Resource;
        }
    }
}