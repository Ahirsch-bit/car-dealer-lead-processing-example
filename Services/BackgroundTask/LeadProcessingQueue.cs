using System.Collections.Concurrent;
using CarDealer.LeadAutomation.Services.Interfaces;

namespace CarDealer.LeadAutomation.Services.BackgroundTask;

public class LeadProcessingQueue: ILeadProcessingQueue
{
    private readonly ConcurrentQueue<(Guid, Func<CancellationToken, Task>)> _workItems = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellationTokens = new();
    private readonly ConcurrentDictionary<Guid, (TaskStatusEnum Status, DateTime Timestamp)> _taskStatuses = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly TimeSpan _retentionPeriod = TimeSpan.FromHours(1);
    
    public Guid QueueLead(Func<CancellationToken, Task> workItem)
    {
        if (workItem == null) throw new ArgumentNullException(nameof(workItem));

        var workItemId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        _cancellationTokens[workItemId] = cts;
        _taskStatuses[workItemId] = (TaskStatusEnum.Queued, DateTime.UtcNow);

        _workItems.Enqueue((workItemId, token => workItem(cts.Token)));
        _signal.Release();
        return workItemId;
    }
    
    public async Task<(Guid, Func<CancellationToken, Task>)> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);

        if (workItem != default)
        {
            _taskStatuses[workItem.Item1] = (TaskStatusEnum.Processing, DateTime.UtcNow);
        }
        return workItem;
    }
    
    public bool CancelLeadProcessing(Guid workItemId)
    {
        if (_cancellationTokens.TryRemove(workItemId, out var cts))
        {
            cts.Cancel();
            _taskStatuses[workItemId] = (TaskStatusEnum.Canceled, DateTime.UtcNow);
            return true;
        }
        return false;
    }
    
    public TaskStatusEnum GetTaskStatus(Guid workItemId)
    {
        if (_taskStatuses.TryGetValue(workItemId, out var entry))
        {
            return entry.Status;
        }
        return TaskStatusEnum.Failed;
    }
    public void MarkTaskCompleted(Guid workItemId)
    {
        _taskStatuses[workItemId] = (TaskStatusEnum.Completed, DateTime.UtcNow);
    }

    public void MarkTaskFailed(Guid workItemId)
    {
        _taskStatuses[workItemId] = (TaskStatusEnum.Failed, DateTime.UtcNow);
    }

    public void MarkTaskCanceled(Guid workItemId)
    {
        _taskStatuses[workItemId] = (TaskStatusEnum.Canceled, DateTime.UtcNow);
    }

    public void CleanupFinishedTasks()
    {
        var threshold = DateTime.UtcNow - _retentionPeriod;

        foreach (var kvp in _taskStatuses)
        {
            if ((kvp.Value.Status == TaskStatusEnum.Completed ||
                 kvp.Value.Status == TaskStatusEnum.Canceled ||
                 kvp.Value.Status == TaskStatusEnum.Failed) &&
                kvp.Value.Timestamp < threshold)
            {
                _taskStatuses.TryRemove(kvp.Key, out _);
                _cancellationTokens.TryRemove(kvp.Key, out _);
            }
        }
    }
}