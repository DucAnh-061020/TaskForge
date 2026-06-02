namespace TaskForge.Domain.ReadModels;
public class ColumnReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskReadModel> Tasks { get; set; } = new();
}