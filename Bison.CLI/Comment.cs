namespace Bison.Cheep;

public record Comment(int ObservationId, string Author, string Message, long Timestamp) : Cheep(Author, Message, Timestamp);