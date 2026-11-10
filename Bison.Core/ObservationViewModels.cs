namespace Bison.Core;

public record ObservationViewModel(string Author, string Message, string Timestamp);
public record ObservationDetailsViewModel(int Id, string Author, string Message, string Timestamp);
public record ProposalViewModel(string Author, int TaxonId, string Timestamp);