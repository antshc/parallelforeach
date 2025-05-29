# 🧪 Experiment Summary: Benchmarking `Parallel.ForEachAsync` vs `Task.WhenAll + SemaphoreSlim` in .NET 8 10 Requests in parallel

## ⚙️ Test Parameters

- **Request count:** 1000
- **Max concurrency:** 10
- **Simulated I/O delay:** 100ms (via local WireMock server)
- **Environment:** .NET 8 Console App, JetBrains Rider Profiler
- **Runs:** 10 iterations

---

## 📊 Results Summary

| Run | `Parallel.ForEachAsync` Time (ms) | Threads Used | `Task.WhenAll + SemaphoreSlim` Time (ms) | Threads Used |
|-----|-----------------------------------|--------------|------------------------------------------|--------------|
| 1   | 11509                             | 16           | 10899                                    | 13           |
| 2   | 10910                             | 15           | 10889                                    | 14           |
| 3   | 10915                             | 13           | 10887                                    | 13           |
| 4   | 10899                             | 11           | 10852                                    | 12           |
| 5   | 10902                             | 14           | 10904                                    | 16           |
| 6   | 10914                             | 14           | 10864                                    | 12           |
| 7   | 10920                             | 13           | 10912                                    | 16           |
| 8   | 10890                             | 15           | 10875                                    | 15           |
| 9   | 10897                             | 12           | 10897                                    | 12           |
| 10  | 10893                             | 13           | 10883                                    | 15           |

---

## 📈 Averages

| Metric                   | `Parallel.ForEachAsync` | `Task.WhenAll + SemaphoreSlim` |
|-------------------------|--------------------------|--------------------------------|
| **Average Time (ms)**   | 10,914.9                 | 10,886.2                       |
| **Average Thread Count**| 13.6                     | 13.8                           |

---

## 🔍 Key Observations

- **Execution Time**: Both methods are highly comparable, with `Task.WhenAll + SemaphoreSlim` being ~0.3% faster on average.
- **Thread Usage**: Thread usage was nearly identical, hovering around 13–14 unique threads in both approaches.


# 🧪 Experiment Summary: `Parallel.ForEachAsync` vs `Task.WhenAll + SemaphoreSlim` in .NET 8 (High Load Test) 1000 Requests in parallel

## ⚙️ Test Parameters

- **Request count:** 100,000
- **Max concurrency:** 1000
- **Simulated I/O delay:** 300ms per HTTP GET request
- **Environment:** .NET 8 Console App, JetBrains Rider Profiler
- **Runs:** 10 iterations

---

## 📊 Results Summary

| Run | `Parallel.ForEachAsync` Time (ms) | Threads Used | `Task.WhenAll + SemaphoreSlim` Time (ms) | Threads Used |
|-----|-----------------------------------|--------------|------------------------------------------|--------------|
| 1   | 32,507                            | 24           | 31,695                                   | 16           |
| 2   | 31,832                            | 14           | 31,707                                   | 15           |
| 3   | 31,455                            | 17           | 31,784                                   | 18           |
| 4   | 31,750                            | 15           | 31,711                                   | 15           |
| 5   | 31,690                            | 17           | 31,618                                   | 17           |
| 6   | 31,669                            | 15           | 31,602                                   | 13           |
| 7   | 31,670                            | 14           | 31,636                                   | 14           |
| 8   | 31,707                            | 13           | 31,617                                   | 14           |
| 9   | 31,788                            | 13           | 31,585                                   | 13           |
| 10  | 31,675                            | 13           | 31,681                                   | 13           |

---

## 📈 Averages

| Metric                   | `Parallel.ForEachAsync` | `Task.WhenAll + SemaphoreSlim` |
|-------------------------|--------------------------|--------------------------------|
| **Average Time (ms)**   | 31,725.3                 | 31,693.6                       |
| **Average Thread Count**| 15.5                     | 14.8                           |

---

## 🔍 Key Observations

- **Execution Time**: `Task.WhenAll + SemaphoreSlim` was slightly faster, averaging ~32 ms less per run (0.1% improvement).
- **Thread Usage**: Both approaches used minimal thread counts (~15 threads) despite 1000 max concurrency, thanks to efficient async I/O handling.

---

## 🧠 Conclusion

- ✅ **Both approaches scale efficiently** even under high load (100k requests, 1000 concurrency).
- ✅ `Task.WhenAll + SemaphoreSlim` maintained a slight advantage in time and stability, especially in controlling concurrency and resource usage.
- ✅ `Parallel.ForEachAsync` provides a more concise syntax and similar performance, making it a great choice for simpler high-load I/O operations.

---

## 💡 Recommendation

| Scenario                                     | Recommended Approach             |
|---------------------------------------------|----------------------------------|
| High-performance I/O (e.g., HTTP, file)      | Both are suitable                |
| Need for custom coordination (timeouts, retries) | `Task.WhenAll + SemaphoreSlim` |
| Simpler implementation, readable code       | `Parallel.ForEachAsync`          |
| CPU-bound workloads                          | Neither – use `Parallel.ForEach` |

---
