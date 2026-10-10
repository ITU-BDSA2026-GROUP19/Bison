namespace Bison.Razor.Tests;

public class TimeLineTests
{
    [Fact]
    public async Task PublicTimelineContainsExampleObservations()
    {
        var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/obs/");

        Assert.Contains("A big gray bird in a pond at DR byen", responseContent);
        Assert.Contains("A heron", responseContent);
    }

    [Fact]
    public async Task PrivateTimelineContainsOnlyUserObservations()
    {
        var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/obs/Peter");

        Assert.Contains("A big gray bird in a pond at DR byen", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
    }

    [Fact]
    public async Task UnknownUsersTimelineShowsNoObservations()
    {
        var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/obs/UnknownUser");

        Assert.Contains("There are no Observations so far.", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
        Assert.DoesNotContain("A big gray bird in a pond at DR byen", responseContent);
    }

    [Fact]
    public async Task UserWithNoObservationsShowsNoObservations()
    {
        var testDatabase = new TestDatabase();
        using var appHelper = new RazorAppHelper(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Bison.Razor"), testDatabase);

        var responseContent = await appHelper.GetAsync("/obs/Paul");

        Assert.Contains("There are no Observations so far.", responseContent);
        Assert.DoesNotContain("A heron", responseContent);
        Assert.DoesNotContain("A big gray bird in a pond at DR byen", responseContent);
    }
}