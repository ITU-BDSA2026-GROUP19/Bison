using Microsoft.Data.Sqlite;

public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
    public List<ObservationViewModel> GetObservations(int page);
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page);
}

public class ObservationService : IObservationService
{
    private const int PageSize = 32;
    private readonly DBFacade _db;

    public ObservationService(DBFacade db)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations(int page)
    {
        int offset = (page - 1) * PageSize;

        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation o
            JOIN user u ON u.user_id = o.author_id
            ORDER BY o.pub_date DESC
            LIMIT @pageSize OFFSET @offset
            """;

        return _db.Query(
            sql, 
            MapRow,
            new SqliteParameter("@pageSize", PageSize),
            new SqliteParameter("@offset", offset));
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page)
    {
        int offset = (page - 1) * PageSize;

        const string sql = """
            SELECT u.username, o.text, o.pub_date
            FROM observation o
            JOIN user u ON u.user_id = o.author_id
            WHERE u.username = @author
            ORDER BY o.pub_date DESC
            LIMIT @pageSize OFFSET @offset
            """;

        return _db.Query(
        sql, 
        MapRow, 
        new SqliteParameter("@author", author),
        new SqliteParameter("@pageSize", PageSize),
        new SqliteParameter("@offset", offset));
    }

    private static ObservationViewModel MapRow(SqliteDataReader reader)
    {
        return new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)));
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}