namespace Bison.Infrastructure;

using Bison.Core;

public static class AchievementInitializer
{
    public static void SeedDatabase(BisonDBContext context)
    {
        if (!context.Achievements.Any())
        {
            var achievementOne = new Achievement() {Name = "Blogger", Description = "Make your own Post"};
            var achievementTwo = new Achievement() {Name =  "Commentator", Description = "Make your first Comment"};
            var achievementThree = new Achievement() {Name =  "Proposer", Description = "Make your first Proposal"};
            var achievementFour = new Achievement() {Name =  "Conversationalist", Description = "Comment on someone elses Post"};
            var achievementFive = new Achievement() {Name =  "TaxonomyHelper", Description = "Propose a Taxon on someone elses Post"};
            var achievementSix = new Achievement() {Name =  "Photographer", Description = "Upload your first Image"};
            var achievementSeven = new Achievement() {Name =  "Completionist", Description = "You Achieved all achievements! what a geek!"};

            List<Achievement> achievements = new List<Achievement> {achievementOne, achievementTwo, achievementThree, achievementFour, achievementFive, achievementSix, achievementSeven};

            context.Achievements.AddRange(achievements);
            context.SaveChanges();
        }
    }
}