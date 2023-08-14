namespace No8.Ascii;

public static class TaskHelpers
{
    private static async Task<T> DelayedTimeoutExceptionTask<T>(TimeSpan delay)
    {
        await Task.Delay(delay);
        throw new TimeoutException();
    }
    
    private static async Task<T?> DelayedTimeoutTask<T>(TimeSpan delay)
    {
        await Task.Delay(delay);
        return default;
    }
    
    /// <summary>
    ///     Starts a task with a timeout. A TimeoutException is thrown if Task is not completed on time
    /// </summary>
    internal static async Task<T> TaskWithTimeoutException<T>(Task<T> task, TimeSpan timeout)
    {
        return await await Task.WhenAny(task, DelayedTimeoutExceptionTask<T>(timeout));
    }
    
    /// <summary>
    ///     Starts a task with a timeout. default value is returned if Task is not completed on time
    /// </summary>
    internal static async Task<T?> TaskWithTimeoutDefault<T>(Task<T> task, TimeSpan timeout)
    {
        return await await Task.WhenAny(task, DelayedTimeoutTask<T>(timeout));
    }
}