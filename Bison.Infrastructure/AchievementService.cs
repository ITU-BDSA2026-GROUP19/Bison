using Bison.Core;

namespace Bison.Infrastructure;

public class AchievementService : IAchievementService
{
    private readonly IAchievementRepository _achievementRepo;
    private readonly IPostRepository _postRepository;
    public AchievementService(IAchievementRepository achievementRepository, IPostRepository postRepository)
    {
        _achievementRepo = achievementRepository;
        _postRepository = postRepository;
    }

    public async Task AwardAchievementsAsync(Author author, List<string> achievementNames)
    {

        var earnedAchievements =
            await _achievementRepo.GetAchievementsByAuthorAsync(author);
        
        foreach (String name in achievementNames)
        {
            if (!earnedAchievements.Any(a => a.Name == name))
            {
                Achievement? achievement = await _achievementRepo.GetAchievementByNameAsync(name);
                if (achievement != null)
                {
                    await _achievementRepo.GrantAchievementAsync(achievement ,author);
                    earnedAchievements.Add(achievement);
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
            earnedAchievements.Any(a => a.Name == name));

        if (hasAllAchievements && !earnedAchievements.Any(a => a.Name == "Completionist"))
        {
            Achievement? achievement = await _achievementRepo.GetAchievementByNameAsync("Completionist");
            if (achievement != null)
            {
                await _achievementRepo.GrantAchievementAsync(achievement, author);
                earnedAchievements.Add(achievement);
            }
        }
    }

    public async Task<List<AchievementViewModel>> GetAchievementsForAuthorAsync(string authorEmail)
    {
        var achievements = await _achievementRepo.GetAllAchievementsAsync();
        var author = await _postRepository.GetAuthorByEmailAsync(authorEmail);
        if (author == null)
        {
            return new List<AchievementViewModel>();
        }

        var earnedAchievements = await _achievementRepo.GetAchievementsByAuthorAsync(author);
        var models = new List<AchievementViewModel> {};

        foreach (Achievement ach in achievements) {
            models.Add(new AchievementViewModel(ach.Name, ach.Description, earnedAchievements.Any(a => a.AchievementId == ach.AchievementId)));
        }

         return models;
    }
}