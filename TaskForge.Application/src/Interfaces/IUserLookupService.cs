namespace TaskForge.Application.Interfaces;

public interface IUserLookupService
{
    Task<Guid?> GetUserIdByEmailAsync(string email);
}