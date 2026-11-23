using Bison.Core;

namespace Bison.Infrastructure;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepo;
    public AchievementService(IAchievementRepository achievementRepository)
    {
        _achievementRepo = achievementRepository;
    }

    public async Task AwardAchievementsAsync(Author author, List<string> achievementNames)
    {
        foreach (String name in achievementNames)
        {
            if (!author.Achievements.Any(a => a.Name == name))
            {
                Achievement? achievement = await _achievementRepo.GetAchievementByNameAsync(name);
                if (achievement != null)
                {
                    await _achievementRepo.GrantAchievementAsync(achievement ,author);
                }
            }
        }

        var requiredAchievements = new List<string>
        {
            "Blogger",
            "Commentator",
            "Proposer",
            "Conversationalist",
            "TaxonomyHelper",
            "Photographer"
        };

        bool hasAllAchievements = requiredAchievements.All(name =>
            author.Achievements.Any(a => a.Name == name));

        if (hasAllAchievements && !author.Achievements.Any(a => a.Name == "Completionist"))
        {
            Achievement? achievement = await _achievementRepo.GetAchievementByNameAsync("Completionist");
            if (achievement != null)
            {
                await _achievementRepo.GrantAchievementAsync(achievement, author);
            }
        }
    }
}