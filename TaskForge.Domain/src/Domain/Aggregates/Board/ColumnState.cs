namespace TaskForge.Domain.Aggregates.Board;

public class ColumnState
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public int Order { get; private set; }

    public ColumnState(Guid id, string name, int order)
    {
        Id = id;
        Name = name;
        Order = order;
    }
}