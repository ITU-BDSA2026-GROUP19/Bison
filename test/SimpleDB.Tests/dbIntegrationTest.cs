namespace SimpleDB.Tests;

public class DbIntegrationTests
{
    public record TestRecord(string Name, string Message, long Timestamp);

    CSVDatabase<TestRecord> database =
        CSVDatabase<TestRecord>.GetInstance("../../../../../data/test.csv");

    [Fact]
    public void Store_Then_Read()
    {
        database.Store(new TestRecord("Goat", "Hello there!", 143421312312));

        IEnumerable<TestRecord> output = database.Read();

        Assert.Contains(output, record => record.Name == "Goat");
    }

    [Fact]
    public void Read_Limit()
    {
        database.Store(new TestRecord("Goat", "Hello there!", 143421312312));
        database.Store(new TestRecord("Goat", "Hello there!", 143421312315));
        database.Store(new TestRecord("Goat", "Hello there!", 143421312314));
        database.Store(new TestRecord("Goat", "Hello there!", 143421312313));

        IEnumerable<TestRecord> output = database.Read(2);

        Assert.Equal(2, output.Count());
    }
}