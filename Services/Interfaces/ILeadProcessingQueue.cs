using CarDealer.LeadAutomation.Services.BackgroundTask;

namespace CarDealer.LeadAutomation.Services.Interfaces;

public interface ILeadProcessingQueue
{
    Guid QueueLead(Func<CancellationToken, Task> workItem);
    Task<(Guid, Func<CancellationToken, Task>)> DequeueAsync(CancellationToken cancellationToken);
    bool CancelLeadProcessing(Guid workItemId);
    TaskStatusEnum GetTaskStatus(Guid workItemId);
    void CleanupFinishedTasks();
    void MarkTaskCanceled(Guid workItemId);
    void MarkTaskFailed(Guid workItemId);
    void MarkTaskCompleted(Guid workItemId);
}