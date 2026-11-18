namespace Bison.Core;

public record ObservationViewModel(int Id, string Author, string AuthorEmail, string Message, string Timestamp, string? PicturePath);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp, string? PicturePath);
public record ProposalViewModel(string Author, int TaxonId, string Timestamp);