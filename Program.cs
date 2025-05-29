using System.Collections.Concurrent;
using System.Diagnostics;
using WireMock.Server;

namespace ParallelForEach.App;

class Program
{
    static async Task Main(string[] args)
    {
        var wireMockServer = WireMockServer.Start();
        // Mock the external Get helloworld API endpoint with status code 200
        wireMockServer.Given(WireMock.RequestBuilders.Request.Create()
                .WithPath("/helloworld")
                .UsingGet())
            .RespondWith(WireMock.ResponseBuilders.Response.Create()
                .WithStatusCode(200)
                .WithBody("Hello World")
                .WithDelay(TimeSpan.FromMilliseconds(300)));

        int requestCount = 100000;
        int maxConcurrency = 1000;

        Console.WriteLine($"Requests count: {requestCount}, Max Concurrency: {maxConcurrency}, Response Time: 300ms");

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("Experiment run: " + (i + 1));
            Console.WriteLine("Running Parallel.ForEachAsync...");
            using (var client = new HttpClient() { BaseAddress = new Uri(wireMockServer.Url) })
            {
                await RunParallelForEachAsync(requestCount, maxConcurrency, client);
            }

            Console.WriteLine("Running Task.WhenAll + SemaphoreSlim...");
            using (var client = new HttpClient() { BaseAddress = new Uri(wireMockServer.Url) })
            {
                await RunTaskWhenAllAsync(requestCount, maxConcurrency, client);
            }
        }
    }

    static async Task RunParallelForEachAsync(int count, int concurrency, HttpClient client)
    {
        var threadIds = new ConcurrentDictionary<int, byte>();
        var sw = Stopwatch.StartNew();

        await Parallel.ForEachAsync(Enumerable.Range(0, count),
            new ParallelOptions()
            {
                MaxDegreeOfParallelism = concurrency
            },
            async (i, ct) =>
            {
                threadIds.TryAdd(Environment.CurrentManagedThreadId, 0);
                await client.GetAsync("/helloworld", ct).ConfigureAwait(false);
                threadIds.TryAdd(Environment.CurrentManagedThreadId, 0);
            });

        sw.Stop();
        Console.WriteLine($"Parallel.ForEachAsync used {threadIds.Count} threads in {sw.ElapsedMilliseconds}ms");
    }

    static async Task RunTaskWhenAllAsync(int count, int concurrency, HttpClient client)
    {
        var threadIds = new ConcurrentDictionary<int, byte>();
        var throttler = new SemaphoreSlim(concurrency);
        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, count).Select(async i =>
        {
            await throttler.WaitAsync();
            try
            {
                threadIds.TryAdd(Environment.CurrentManagedThreadId, 0);
                await client.GetAsync("/helloworld").ConfigureAwait(false);
                threadIds.TryAdd(Environment.CurrentManagedThreadId, 0);
            }
            finally
            {
                throttler.Release();
            }
        });

        await Task.WhenAll(tasks);
        sw.Stop();
        Console.WriteLine($"Task.WhenAll used {threadIds.Count} threads in {sw.ElapsedMilliseconds}ms");
    }
}