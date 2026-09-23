namespace Bison.Cheep;

public record Observation(int Id, string Author, string Message, long Timestamp) : Cheep(Author, Message, Timestamp);