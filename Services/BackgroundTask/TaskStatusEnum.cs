namespace CarDealer.LeadAutomation.Services.BackgroundTask;

public enum TaskStatusEnum
{
    Queued, 
    Processing,
    Completed, 
    Canceled, 
    Failed
}