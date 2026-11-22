namespace Bison.Core;

public class Achievement
{
    public int AchievementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Author> Authors { get; set; } = new();
}