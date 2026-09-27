namespace Bison.Cheep;

public record Observation(int Id, string Author, string Message, long Timestamp, string Location) : Cheep(Author, Message, Timestamp);