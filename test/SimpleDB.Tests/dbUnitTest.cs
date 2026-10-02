namespace SimpleDB.Tests;

public class DbTest
{
    [Fact]
    public void TestDatabase()
    {
        var path = Path.GetTempFileName();
        try
        {
            var database = CSVDatabase<TestRow>.GetInstance(path);
            database.store(new TestRow { name = "bison", count = 2 });

            var result = Assert.Single(database.read());

            Assert.Equal("bison", result.name);
            Assert.Equal(2, result.count);
        }
        finally
        {
            File.Delete(path);
        }
    }

    public sealed class TestRow
    {
        public string name { get; set; } = "";
        public int count { get; set; }
    }
}
