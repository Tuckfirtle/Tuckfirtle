using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheDialgaTeam.Core.DependencyInjection
{
    public class TaskAwaiter : IDisposable
    {
        private CancellationTokenSource CancellationTokenSource { get; }

        private List<Task> TaskToAwait { get; } = new List<Task>();

        public TaskAwaiter(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public Task EnqueueTask(Task taskToAwait)
        {
            TaskToAwait.Add(taskToAwait);
            return taskToAwait;
        }

        public Task EnqueueTask(Func<CancellationToken, Action> taskToAwait)
        {
            var task = Task.Run(taskToAwait(CancellationTokenSource.Token), CancellationTokenSource.Token);
            TaskToAwait.Add(task);
            return task;
        }

        public void Clear()
        {
            TaskToAwait.Clear();
        }

        public void WaitAll()
        {
            Task.WaitAll(TaskToAwait.ToArray(), CancellationTokenSource.Token);
        }

        public void Dispose()
        {
            foreach (var task in TaskToAwait)
                task.Dispose();

            TaskToAwait.Clear();
        }
    }
}