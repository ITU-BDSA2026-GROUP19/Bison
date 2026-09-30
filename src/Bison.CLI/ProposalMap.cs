using CsvHelper.Configuration;

namespace Bison.Cheep;

public sealed class ProposalMap : ClassMap<Proposal>
{
    public ProposalMap()
    {
        // ---- Mapping properties of the Proposal class to CSV columns
        Map(m => m.ObservationId).Name("ObservationId");
        Map(m => m.Author).Name("Author");
        Map(m => m.TaxonId).Name("TaxonId");
        Map(m => m.Timestamp).Name("Timestamp");

        // ---- Mapping constructor parameters of the Proposal record to CSV columns
        Parameter("ObservationId").Name("ObservationId");
        Parameter("Author").Name("Author");
        Parameter("TaxonId").Name("TaxonId");
        Parameter("Timestamp").Name("Timestamp");
    }
}