using System.Diagnostics;

namespace Core.Infrastructure.Utils;

public static class TimeTracker
{
  public static async Task<(TResult result, long timeElapsedInMs)> Execute<TResult>(
    Func<Task<TResult>> function
  )
  {
    var stopwatch = new Stopwatch();

    stopwatch.Start();

    var result = await function();

    stopwatch.Stop();

    return (result, stopwatch.ElapsedMilliseconds);
  }
}
