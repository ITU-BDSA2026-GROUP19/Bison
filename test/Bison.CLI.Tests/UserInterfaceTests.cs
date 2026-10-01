using Bison.Cheep;

namespace Bison.CLI.Tests;

public class UserInterfaceTests
{
    [Fact]
    public void PrintCheeps_ConvertsTimestampCorrectly()
    {
        var cheep = new Observation(1, "Alice", "Hello", 0, "Copenhagen");

        using var writer = new StringWriter();
        Console.SetOut(writer);

        UserInterface.PrintCheeps(new[] { cheep });

        string output = writer.ToString();

        Assert.Contains("Alice @ 01/01/70 00:00:00: Hello", output);
    }

    [Fact]
    public void PrintObservations_PrintsObservationCorrectly()
    {
        var observation = new Observation(1, "Alice", "Hello", 0, "Copenhagen");

        using var writer = new StringWriter();
        Console.SetOut(writer);

        UserInterface.PrintObservations(new[] { observation });

        string output = writer.ToString();

        Assert.Contains("1: Alice @ 01/01/70 00:00:00: Hello: Copenhagen", output);
    }

    [Fact]
    public void PrintComments_PrintsCommentCorrectly()
    {
        var comment = new Comment(1, "Alice", "Nice observation", 0);

        using var writer = new StringWriter();
        Console.SetOut(writer);

        UserInterface.PrintComments(new[] { comment });

        string output = writer.ToString();

        Assert.Contains("Alice @ 01/01/70 00:00:00: Nice observation", output);
    }
}
