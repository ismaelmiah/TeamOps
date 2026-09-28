using Microsoft.EntityFrameworkCore;
using TeamOps.Application.Projects;
using TeamOps.Infrastructure.Persistence;
using TeamOps.Infrastructure.Persistence.Projects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<CreateProjectHandler>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddDbContext<TeamOpsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TeamOps")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

public partial class Program
{
}