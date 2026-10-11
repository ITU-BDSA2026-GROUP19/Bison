using Microsoft.Data.Sqlite;

public record ObservationViewModel(string Author, string Message, string Timestamp);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp);
public record ProposalViewModel(string Author, int TaxonId, string Timestamp);

public interface IObservationService
{
    public Task<List<ObservationViewModel>> GetObservations(int page);
    public Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page);

    public Task<List<ObservationDetailsViewModel>> GetObservationById(int id);
    public Task<List<ObservationViewModel>> GetComments(int observationId);
    public Task<List<ProposalViewModel>> GetProposals(int observationId);
}

public class ObservationService : IObservationService
{
    private const int PageSize = 32;
    //private readonly DBFacade _db;
    private IPostRepository _postRepository;

    //public ObservationService(DBFacade db)
    public ObservationService(IPostRepository db)
    {
        _postRepository = db;
    }

    //public List<ObservationViewModel> GetObservations(int page)
    public async Task<List<ObservationViewModel>> GetObservations(int page)
    {
        int offset = (page - 1) * PageSize;

        /*const string sql = """
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
            new SqliteParameter("@offset", offset));*/
            List<Observation> observations = await _postRepository.GetPostsByPageAsync(page, PageSize);
            return observations.Select(o => new ObservationViewModel(
                o.Author.Name,
                o.Text,
                o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            )).ToList();
    }

    public async Task<List<ObservationViewModel>> GetObservationsFromAuthor(string author, int page)
    {
        /*int offset = (page - 1) * PageSize;

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
            new SqliteParameter("@offset", offset));*/
        List<Observation> observations = await _postRepository.GetPostsByAuthorAsync(author, page, PageSize);
        return observations.Select(o => new ObservationViewModel(
            o.Author.Name,
            o.Text,
            o.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
    }

    public async Task<List<ObservationDetailsViewModel>> GetObservationById(int id)
    {
        /*const string sql = """
            SELECT o.observation_id, u.username, o.text, o.pub_date
            FROM observation o
            JOIN user u ON u.user_id = o.author_id
            WHERE o.observation_id = @id
            """;

        return _db.Query(
            sql,
            MapDetailsRow,
            new SqliteParameter("@id", id));*/
        var observation = await _postRepository.GetObservationByIdAsync(id);
        if (observation == null)
        {
            return new List<ObservationDetailsViewModel>();
        }

        return new List<ObservationDetailsViewModel>
        {
            new ObservationDetailsViewModel(
                observation.PostId,
                observation.Author.Name,
                observation.Text,
                observation.TimeStamp.ToString("MM/dd/yy H:mm:ss")
            )
        };
    }

    public async Task<List<ObservationViewModel>> GetComments(int observationId)
    {
        /*const string sql = """
            SELECT u.username, c.text, c.pub_date
            FROM comment c
            JOIN user u ON u.user_id = c.author_id
            WHERE c.observation_id = @observationId
            ORDER BY c.pub_date ASC
            """;

        return _db.Query(
            sql,
            MapRow,
            new SqliteParameter("@observationId", observationId));*/
        var comments = await _postRepository.GetCommentsByObservationIdAsync(observationId);
        return comments.Select(c => new ObservationViewModel(
            c.Author.Name,
            c.Text,
            c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
    }

    public async Task<List<ProposalViewModel>> GetProposals(int observationId)
    {
        /*const string sql = """
            SELECT u.username, p.taxon_id, p.pub_date
            FROM proposal p
            JOIN user u ON u.user_id = p.author_id
            WHERE p.observation_id = @observationId
            ORDER BY p.pub_date ASC
            """;

        return _db.Query(
            sql,
            MapProposalRow,
            new SqliteParameter("@observationId", observationId));*/
        var proposals = await _postRepository.GetProposalsByObservationIdAsync(observationId);
        return proposals.Select(p => new ProposalViewModel(
            p.Author.Name,
            p.Taxon.TaxonId,
            p.TimeStamp.ToString("MM/dd/yy H:mm:ss")
        )).ToList();
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
            reader.GetInt32(1),
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