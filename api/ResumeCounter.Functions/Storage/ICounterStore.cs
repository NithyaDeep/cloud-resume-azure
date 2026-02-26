namespace ResumeCounter.Functions.Storage;

public interface ICounterStore
{
    Task<int> GetAsync(CancellationToken ct);
    Task<int> IncrementAsync(CancellationToken ct);
}