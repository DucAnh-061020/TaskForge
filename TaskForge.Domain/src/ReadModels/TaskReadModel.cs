namespace TaskForge.Domain.ReadModels;
public class TaskReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AssignedTo { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid ColumnId { get; set; }
    public Guid BoardId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}