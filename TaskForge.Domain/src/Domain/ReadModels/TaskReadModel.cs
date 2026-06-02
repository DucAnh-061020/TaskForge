namespace TaskForge.Domain.ReadModels;
public class TaskReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}