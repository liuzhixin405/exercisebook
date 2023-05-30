using System.Collections.Concurrent;

internal sealed class DedicatedThreadTaskScheduler : TaskScheduler
{
    private readonly BlockingCollection<Task> _tasks = new();
    private readonly Thread[] _threads;
    
    protected override IEnumerable<Task>? GetScheduledTasks()
    =>_tasks;

    protected override void QueueTask(Task task)=>_tasks.Add(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    =>false;

    public DedicatedThreadTaskScheduler(int threadCount)
    {
        _threads= new Thread[threadCount];
        for(int index=0;index<threadCount;index++){
            _threads[index] = new Thread(_=>{
                while(true){
                    TryExecuteTask(_tasks.Take());
                }
            });
        }
        Array.ForEach(_threads,it=>it.Start());
    }
}