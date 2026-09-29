using CsvHelper.Configuration;

namespace Bison.Cheep;

public sealed class ProposalMap : ClassMap<Proposal>
{
    public ProposalMap()
    {
        Map(m => m.ObservationId).Name("ObservationId");
        Map(m => m.Author).Name("Author");
        Map(m => m.TaxonId).Name("TaxonId");
        Map(m => m.Timestamp).Name("Timestamp");

        Parameter("ObservationId").Name("ObservationId");
        Parameter("Author").Name("Author");
        Parameter("TaxonId").Name("TaxonId");
        Parameter("Timestamp").Name("Timestamp");
    }
}