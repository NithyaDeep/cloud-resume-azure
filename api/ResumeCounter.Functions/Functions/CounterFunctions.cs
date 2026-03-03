using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using ResumeCounter.Functions.Storage;


namespace ResumeCounter.Functions.Functions;

public class CounterFunctions
{
    private readonly ICounterStore _store;

    public CounterFunctions(ICounterStore store)
    {
        _store = store;
    }

    [Function("GetCounter")]
public async Task<HttpResponseData> GetCounter(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "counter")] HttpRequestData req,
    CancellationToken ct)
{
    var count = await _store.GetAsync(ct);
    return await JsonAsync(req, new { count }, ct);
}

[Function("IncrementCounter")]
public async Task<HttpResponseData> IncrementCounter(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "counter/increment")] HttpRequestData req,
    CancellationToken ct)
{
    var count = await _store.IncrementAsync(ct);
    return await JsonAsync(req, new { count }, ct);
}

[Function("Health")]
public async Task<HttpResponseData> Health(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req,
    CancellationToken ct)
{
    //adding comment
    var res = req.CreateResponse(HttpStatusCode.OK);
    await res.WriteAsJsonAsync(new { status = "ok" }, cancellationToken: ct);
    return res;
}

private static async Task<HttpResponseData> JsonAsync(HttpRequestData req, object payload, CancellationToken ct)
{
    var res = req.CreateResponse(HttpStatusCode.OK);
    await res.WriteAsJsonAsync(payload, cancellationToken: ct);
    return res;
}
}