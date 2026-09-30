using Microsoft.EntityFrameworkCore;
using TeamOps.Application.Projects;
using TeamOps.Application.Tasks;
using TeamOps.Application.Users;
using TeamOps.Infrastructure.Persistence;
using TeamOps.Infrastructure.Persistence.Projects;
using TeamOps.Infrastructure.Persistence.Tasks;
using TeamOps.Infrastructure.Persistence.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<CreateProjectHandler>();
builder.Services.AddScoped<GetProjectHandler>();
builder.Services.AddScoped<GetProjectsHandler>();
builder.Services.AddScoped<CompleteProjectHandler>();
builder.Services.AddScoped<AddProjectMemberHandler>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<RemoveProjectMemberHandler>();
builder.Services.AddScoped<CreateTaskHandler>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

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