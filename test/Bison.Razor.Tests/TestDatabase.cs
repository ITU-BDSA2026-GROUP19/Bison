using Microsoft.Data.Sqlite;

public class TestDatabase
{
    public string DbPath { get; }

    public TestDatabase()
{
    DbPath = Path.Combine(Path.GetTempPath(), $"bison-test-{Guid.NewGuid()}.db");

    var schemaPath = Path.Combine(AppContext.BaseDirectory, "Schema.sql");
    var schemaSql = File.ReadAllText(schemaPath);

    using var connection = new SqliteConnection($"Data Source={DbPath}");
    connection.Open();
    using var command = connection.CreateCommand();
    command.CommandText = schemaSql;
    command.ExecuteNonQuery();

    using var insertCommand = connection.CreateCommand();
    insertCommand.CommandText = @"
        INSERT INTO user (user_id, username, email, pw_hash) VALUES
        (1, 'Peter', 'peter@example.com', 'hashed_password_1'),
        (2, 'Petra', 'petra@example.com', 'hashed_password_2'),
        (3, 'Paul', 'paul@example.com', 'hashed_password_3')
    ";
    insertCommand.ExecuteNonQuery();
    using var insertCommand2 = connection.CreateCommand();
    insertCommand2.CommandText = @"
        INSERT INTO observation (observation_id, author_id, text, pub_date) VALUES
        (1, 1, 'A big gray bird in a pond at DR byen', '1672531200'),
        (2, 2, 'A heron', '1672617600')";
    insertCommand2.ExecuteNonQuery();
}
}
