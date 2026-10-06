using Microsoft.Data.Sqlite;

public record ObservationViewModel(string Author, string Message, string Timestamp);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp);
public record ProposalViewModel(string Author, string TaxonId, string Timestamp);

public interface IObservationService
{
    public List<ObservationViewModel> GetObservations(int page);
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page);

    public List<ObservationDetailsViewModel> GetObservationById(int id);
    public List<ObservationViewModel> GetComments(int observationId);
    public List<ProposalViewModel> GetProposals(int observationId);
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
            ORDER BY o.pub_date DESC, o.observation_id DESC
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
            ORDER BY o.pub_date DESC, o.observation_id DESC
            LIMIT @pageSize OFFSET @offset
            """;

        return _db.Query(
            sql,
            MapRow,
            new SqliteParameter("@author", author),
            new SqliteParameter("@pageSize", PageSize),
            new SqliteParameter("@offset", offset));
    }

    public List<ObservationDetailsViewModel> GetObservationById(int id)
    {
        const string sql = """
            SELECT o.observation_id, u.username, o.text, o.pub_date
            FROM observation o
            JOIN user u ON u.user_id = o.author_id
            WHERE o.observation_id = @id
            """;

        return _db.Query(
            sql,
            MapDetailsRow,
            new SqliteParameter("@id", id));
    }

    public List<ObservationViewModel> GetComments(int observationId)
    {
        const string sql = """
            SELECT u.username, c.text, c.pub_date
            FROM comment c
            JOIN user u ON u.user_id = c.author_id
            WHERE c.observation_id = @observationId
            ORDER BY c.pub_date ASC
            """;

        return _db.Query(
            sql,
            MapRow,
            new SqliteParameter("@observationId", observationId));
    }

    public List<ProposalViewModel> GetProposals(int observationId)
    {
        const string sql = """
            SELECT u.username, p.taxon_id, p.pub_date
            FROM proposal p
            JOIN user u ON u.user_id = p.author_id
            WHERE p.observation_id = @observationId
            ORDER BY p.pub_date ASC
            """;

        return _db.Query(
            sql,
            MapProposalRow,
            new SqliteParameter("@observationId", observationId));
    }

    private static ObservationViewModel MapRow(SqliteDataReader reader)
    {
        return new ObservationViewModel(
            reader.GetString(0),
            reader.GetString(1),
            UnixTimeStampToDateTimeString(reader.GetInt64(2)));
    }

    private static ObservationDetailsViewModel MapDetailsRow(SqliteDataReader reader)
    {
        return new ObservationDetailsViewModel(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            UnixTimeStampToDateTimeString(reader.GetInt64(3)));
    }

    private static ProposalViewModel MapProposalRow(SqliteDataReader reader)
    {
        return new ProposalViewModel(
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