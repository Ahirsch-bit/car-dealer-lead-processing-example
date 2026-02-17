using CarDealer.LeadAutomation.Contracts.Validators;
using CarDealer.LeadAutomation.Repository;
using CarDealer.LeadAutomation.Repository.Interfaces;
using CarDealer.LeadAutomation.Services;
using CarDealer.LeadAutomation.Services.BackgroundTask;
using CarDealer.LeadAutomation.Services.Interfaces;
using CarDealer.LeadAutomation.Services.LeadEnrichment;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<LeadValidator>();
builder.Services.AddSingleton<IBranchRepository, BranchRepository>();
builder.Services.AddSingleton<IModelRepository, ModelRepository>();
builder.Services.AddSingleton<IEmailValidator, EmailValidator>();
builder.Services.AddSingleton<ILeadEnrichmentRequest, LeadEnrichmentRequest>();
builder.Services.AddSingleton<ILeadsSerivice, LeadsSerivce>();
builder.Services.AddSingleton<ILeadProcessingQueue, LeadProcessingQueue>();
builder.Services.AddHostedService<LeadQueueService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); 
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
