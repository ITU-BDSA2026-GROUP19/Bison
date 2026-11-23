namespace Bison.Infrastructure;

using Bison.Core;

public interface IAchievementRepository
{
    Task<Achievement?> GetAchievementByNameAsync(string name);
    Task <List<Achievement>> GetAchievementsByAuthorAsync(Author author);
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task GrantAchievementAsync(Achievement achievement, Author author);
}