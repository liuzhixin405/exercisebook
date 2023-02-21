using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp8
{
    internal static class TaskExtentions
    {
        public static async Task LogException(this Task task, ILogger logger, int errorCode, string message)
        {
            try
            {
                await task;
            } catch (Exception ex)
            {
                logger.LogError(errorCode, ex, "{Message}", message);
                throw;
            }
        }
        public static async Task SafeExecute(Func<Task> action)
        {
            await action();
        }

        public static async Task ExecuteAndIgnoreException(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception)
            {
                // dont re-throw, just eat it.
            }
        }

        internal static string ToString(this Task t) => t == null ? "null" : $"[Id={t.Id}, Status={t.Status}]";

        public static void WaitWithThrow(this Task task, TimeSpan timeout)
        {
            if (!task.Wait(timeout))
            {
                throw new TimeoutException($"Task.WaitWithThrow has timed out after {timeout}.");
            }
        }
        internal static T WaitForResultWithThrow<T>(this Task<T> task, TimeSpan timeout)
        {
            if (!task.Wait(timeout))
            {
                throw new TimeoutException($"Task<T>.WaitWithThrow has timed out after {timeout}.");
            }
            return task.Result;
        }

        public static async Task WithTimeout(this Task taskComplete, TimeSpan timeSpan, string exceptionMessage = null)
        {
            if (taskComplete.IsCompleted)
            {
                await taskComplete;
                return;
            }
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(taskComplete, Task.Delay(timeSpan, timeoutCancellationTokenSource.Token));
            if (taskComplete == completedTask)
            {
                timeoutCancellationTokenSource.Cancel();
                await taskComplete;
                return;
            }
            taskComplete.Ignore();
            var errorMessage = exceptionMessage ?? $"WithTimeout has timed out after {timeSpan}";
            throw new TimeoutException(errorMessage);
        }
        public static async Task<T> WithTimeout<T>(this Task<T> taskToComplete, TimeSpan timeSpan, string exceptionMessage = null)
        {
            if (taskToComplete.IsCompleted)
            {
                return await taskToComplete;
            }

            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(taskToComplete, Task.Delay(timeSpan, timeoutCancellationTokenSource.Token));

            // We got done before the timeout, or were able to complete before this code ran, return the result
            if (taskToComplete == completedTask)
            {
                timeoutCancellationTokenSource.Cancel();
                // Await this so as to propagate the exception correctly
                return await taskToComplete;
            }

            // We did not complete before the timeout, we fire and forget to ensure we observe any exceptions that may occur
            taskToComplete.Ignore();
            var errorMessage = exceptionMessage ?? $"WithTimeout has timed out after {timeSpan}";
            throw new TimeoutException(errorMessage);
        }

        internal static async Task WithCancellation(this Task taskToComplete, CancellationToken cancellationToken, string message)
        {
            try
            {
                await taskToComplete.WithCancellation(cancellationToken);

            } catch (TaskCanceledException ex)
            {
                throw new TaskCanceledException(message, ex);
            }
        }
        internal static Task WithCancellation(this Task taskToComplete, CancellationToken cancellationToken)
        {
            if (taskToComplete.IsCanceled || !cancellationToken.CanBeCanceled)
            {
                return taskToComplete;
            }else if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<object>(cancellationToken);
            }
            else
            {
                return MakeCancellable(taskToComplete, cancellationToken);
            }
        }
        private static async Task MakeCancellable(Task task,CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken), false))
            {
                var firstToComplete = await Task.WhenAny(task,tcs.Task).ConfigureAwait(false);
                if (firstToComplete != task)
                {
                    task.Ignore();
                }
                await firstToComplete.ConfigureAwait(false);
            }
        }
        internal static Task WrapInTask(Action action)
        {
            try
            {
                action();
                return Task.CompletedTask;
            }catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }
        internal static T GetResult<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }
        internal static Task WhenCancelled(this CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return Task.CompletedTask;
            var waitForCancellation = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            token.Register(obj =>
            {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForCancellation);

            return waitForCancellation.Task;
        }
    }

    public static class PublicTaskExtensions
    {
        private static readonly Action<Task> IgnoreTaskContinuation = t => { _ = t.Exception; };

        /// <param name="task">The task to be ignored.</param>
        public static void Ignore(this Task task)
        {
            if (task.IsCompleted)
            {
                _ = task.Exception;
            }
            else
            {
                task.ContinueWith(
                    IgnoreTaskContinuation,
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }
    }
}