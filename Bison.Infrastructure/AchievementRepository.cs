namespace Bison.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Bison.Core;

public class AchievementRepository : IAchievementRepository
{
    private readonly BisonDBContext _context;

    public AchievementRepository(BisonDBContext context)
    {
        _context = context;
    }

    public async Task<Achievement?> GetAchievementByNameAsync(string name)
    {
        return await _context.Achievements
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<List<Achievement>> GetAchievementsByAuthorAsync(Author author)
    {
        return await _context.Achievements
            .Where(a => a.Authors.Contains(author))
            .ToListAsync();
    }

    public async Task GrantAchievementAsync(Achievement achievement, Author author)
    {
        achievement.Authors.Add(author);
        author.Achievements.Add(achievement);
        await _context.SaveChangesAsync();
    }
}