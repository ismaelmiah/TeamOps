namespace TeamOps.Domain.Tasks;

public sealed class TaskItem
{
    private TaskItem(Guid id, Guid projectId, string title, TaskStatus status)
    {
        Id = id;
        ProjectId = projectId;
        Title = title;
        Status = status;
    }

    public Guid Id { get; }

    public Guid ProjectId { get; }

    public string Title { get; }
    public TaskStatus Status { get; private set; }

    public static TaskItem Create(Guid id, Guid projectId, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title is required.", nameof(title));

        return new TaskItem(id, projectId, title.Trim(), TaskStatus.Pending);
    }

    public void Start()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Completed task cannot be started.");

        Status = TaskStatus.InProgress;
    }

    public void Hold()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Completed task cannot be put on hold.");

        Status = TaskStatus.OnHold;
    }

    public void Complete()
    {
        if (Status != TaskStatus.InProgress)
            throw new InvalidOperationException("Only an in-progress task can be completed.");

        Status = TaskStatus.Completed;
    }
}