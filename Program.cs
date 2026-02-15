using CarDealer.LeadAutomation.Contracts.Validators;
using CarDealer.LeadAutomation.Repository;
using CarDealer.LeadAutomation.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IBranchRepository, BranchRepository>();
builder.Services.AddSingleton<IModelRepository, ModelRepository>();
builder.Services.AddSingleton<IEmailValidator, EmailValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build(); 
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
