using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string _connectionString;

    public DBFacade(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    // Runs a SQL query and converts each row using the "map" function.
    public List<T> Query<T>(string sql, Func<SqliteDataReader, T> map, params SqliteParameter[] parameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);

        using var reader = command.ExecuteReader();
        var results = new List<T>();
        while (reader.Read())
        {
            results.Add(map(reader));
        }
        return results;
    }
}