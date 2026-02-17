using CarDealer.LeadAutomation.Services.Interfaces;

namespace CarDealer.LeadAutomation.Services;

public class LeadQueueService:BackgroundService 
{
    private readonly ILeadProcessingQueue _taskQueue; 
    private readonly ILogger _logger;
    
    public LeadQueueService(ILeadProcessingQueue taskQueue, ILogger<LeadQueueService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Task Queue Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var (workItemId, workItem) = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
                _taskQueue.MarkTaskCompleted(workItemId);
                _logger.LogInformation("Work item {WorkItemId} completed successfully.", workItemId);
            }
            catch (OperationCanceledException ex)
            {
                _taskQueue.MarkTaskCanceled(workItemId);
                _logger.LogWarning(ex, "Work item {WorkItemId} was canceled.", workItemId);
            }
            catch (Exception ex)
            {
                _taskQueue.MarkTaskFailed(workItemId);
                _logger.LogError(ex, "Error occurred executing background work item {WorkItemId}.", workItemId);
            }
            finally
            {
                var finalStatus = _taskQueue.GetTaskStatus(workItemId);
                _logger.LogInformation("Work item {WorkItemId} completed with status {Status}.", workItemId, finalStatus);
            }

            _taskQueue.CleanupFinishedTasks();
        }
    }
}