# Prompt
```text
Generate a C# .NET 8 console app that benchmarks the difference in thread usage and execution time between:
1. Parallel.ForEachAsync
2. Task.WhenAll + SemaphoreSlim

Simulate 60,000 HTTP GET requests to a local endpoint using WireMockServer, delaying each response by 100ms. 

For both approaches:
- Limit maximum concurrency to 1000
- Track the number of unique threads used (Environment.CurrentManagedThreadId)
- Measure and print total elapsed time (using Stopwatch)
- Print thread count and execution time per approach

Use HttpClient with BaseAddress set to WireMockServer.Url.
Use ConfigureAwait(false) in awaits for HTTP client requests.
Structure the test clearly, using separate methods for each strategy.
```
