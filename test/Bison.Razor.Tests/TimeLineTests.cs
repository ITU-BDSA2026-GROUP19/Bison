namespace Bison.Razor.Tests;

public class TimeLineTests
{
    [Fact]
    public async Task PublicTimelineContainsExampleObservations()
    {
        using var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/");

        Assert.Contains("A big gray bird in a pond at DR byen", responseContent);
        Assert.Contains("A heron", responseContent);
    }

    [Fact]
    public async Task PrivateTimelineContainsOnlyUserObservations()
    {
        using var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/Peter");

        Assert.Contains("A big gray bird in a pond at DR byen", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
    }

    [Fact]
    public async Task UnknownUsersTimelineShowsNoObservations()
    {
        using var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/UnknownUser");

        Assert.Contains("There are no Observations so far.", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
        Assert.DoesNotContain("A big gray bird in a pond at DR byen", responseContent);
    }

    [Fact]
    public async Task UserWithNoObservationsShowsNoObservations()
    {
        using var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/Paul");

        Assert.Contains("There are no Observations so far.", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
        Assert.DoesNotContain("A big gray bird in a pond at DR byen", responseContent);
    }
}