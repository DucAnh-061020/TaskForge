namespace TaskForge.Domain.ReadModels;
public class BoardReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
}