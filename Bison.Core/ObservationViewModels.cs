namespace Bison.Core;

public record ObservationViewModel(int Id, string Author, string AuthorEmail, string Message, string Timestamp, string? PicturePath, string? PictureLicense);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp, string? PicturePath, string? PictureLicense);
public record ProposalViewModel(string Author, int TaxonId, string TaxonName, string Timestamp);