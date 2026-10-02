using System.Net;
using System.Net.Http.Json;
using TeamOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamOps.Domain.Users;

namespace TeamOps.Api.Tests.Projects;

public class DeleteProjectApiTests : IClassFixture<TeamOpsApiFactory>
{
    private readonly HttpClient _client;
    private readonly TeamOpsApiFactory _factory;

    public DeleteProjectApiTests(TeamOpsApiFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task DeleteProject_ShouldRemoveProject()
    {
        // create project
        var tenantId = Guid.NewGuid();

        var request = new
        {
            TenantId = tenantId,
            Name = "Project Alpha"
        };

        var response = await _client.PostAsJsonAsync("/api/projects", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal(tenantId, project.TenantId);
        Assert.Equal("Project Alpha", project.Name);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        var savedProject = await context.Projects.SingleAsync(x => x.Id == project!.Id);

        Assert.Equal(project.TenantId, savedProject.TenantId);
        Assert.Equal(project.Name, savedProject.Name);


        var deleteResponse = await _client.DeleteAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // verify project no longer exists in PostgreSQL
        var deletedProject = context.Projects.SingleOrDefault(x => x.Id == project.Id);

        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteProject_ShouldCascadeDeleteTasksAndMembers()
    {
        // Arrange
        // Create project
        var tenantId = Guid.NewGuid();

        var projectResponse = await _client.PostAsJsonAsync(
        "/api/projects",
        new
        {
            TenantId = tenantId,
            Name = "Project Alpha"
        });

        Assert.Equal(HttpStatusCode.Created, projectResponse.StatusCode);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        Assert.NotNull(project);

        // Create task
        var taskResponse = await _client.PostAsJsonAsync(
            "/api/tasks",
            new
            {
                ProjectId = project.Id,
                Title = "Project task"
            });

        Assert.Equal(HttpStatusCode.Created, taskResponse.StatusCode);

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(task);

        // Create user
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/users");

        request.Headers.Add("X-Test-Tenant", tenantId.ToString());

        request.Content = JsonContent.Create(new
        {
            email = "user@example.com",
            name = "Test User",
            Role = UserRole.Member
        });

        var userResponse = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, userResponse.StatusCode);

        var user = await userResponse.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        // Add user to project.
        var memberResponse = await _client.PostAsync(
            $"/api/projects/{project.Id}/members/{user.Id}",
            content: null);

        Assert.Equal(HttpStatusCode.NoContent, memberResponse.StatusCode);

        // Act
        var deleteProject = await _client.DeleteAsync($"/api/projects/{project.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteProject.StatusCode);

        // Assert database state
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeamOpsDbContext>();

        Assert.Null(await context.Projects.SingleOrDefaultAsync(x => x.Id == project.Id));

        Assert.Null(await context.Tasks.SingleOrDefaultAsync(x => x.Id == task.Id));

        Assert.Null(await context.ProjectMembers.SingleOrDefaultAsync(x => x.ProjectId == project.Id && x.UserId == user.Id));

        // Act
        // DELETE /api/projects/{projectId}

        // Assert
        // 204 NoContent
        // project does not exist
        // task does not exist
        // project member does not exist
    }
}