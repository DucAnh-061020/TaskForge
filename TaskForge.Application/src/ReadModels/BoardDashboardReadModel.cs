namespace TaskForge.Application.ReadModels;

public class BoardDashboardReadModel
{
    public Guid Id { get; set; } // Maps to BoardId
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public List<ColumnDocument> Columns { get; set; } = new();
}

public class ColumnDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<TaskDocument> Tasks { get; set; } = new();
}

public class TaskDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AssigneeInfo AssignedTo { get; set; } = new();
    public DateTime LastModified { get; set; }
}

public class AssigneeInfo
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}