namespace TaskForge.Application.ReadModels;

public class UserWorkspaceReadModel
{
    public Guid Id { get; set; } // Maps to UserId
    public string UserEmail { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<ProjectSummary> Workspaces { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class ProjectSummary
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
    public List<BoardSummary> Boards { get; set; } = new();
}

public class BoardSummary
{
    public Guid BoardId { get; set; }
    public string BoardName { get; set; } = string.Empty;
}