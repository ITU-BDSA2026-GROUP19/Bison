using Bison.Core;

public interface IAchievementService
{
    Task AwardAchievementsAsync(Author author, List<string> achievementNames);
}