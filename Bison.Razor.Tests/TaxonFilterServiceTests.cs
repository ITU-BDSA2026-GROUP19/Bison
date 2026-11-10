using Bison.Model;

namespace Bison.Razor.Tests;

public class TaxonFilterServiceTests
{
    [Fact]
    public void Filter_IncludesObservationWithSelectedTaxon()
    {
        // Arrange
        Taxon heron = new Taxon
        {
            TaxonId = 1,
            dwc_TaxonID = "heron",
            VernacularName = "Heron"
        };

        Observation observation = new Observation
        {
            Text = "A heron",
            Taxon = heron
        };

        TaxonFilterService filterService = new TaxonFilterService();
        List<Observation> filteredObservations = filterService.Filter(heron, new List<Observation> { observation });

        Assert.Single(filteredObservations);
        Assert.Contains(observation, filteredObservations);
    }

    [Fact]
    public void Filter_IncludesObservationsBelongingToSubTaxa()
    {
        Taxon bird = new Taxon { TaxonId = 1, dwc_TaxonID = "bird" };
        Taxon heron = new Taxon { TaxonId = 2, dwc_TaxonID = "heron", Parent = bird };

        Observation observation = new Observation
        {
            Text = "A heron",
            Taxon = heron
        };

        TaxonFilterService filterService = new TaxonFilterService();
        List<Observation> filteredObservations = filterService.Filter(bird, new List<Observation> { observation });

        Assert.Single(filteredObservations);
        Assert.Contains(observation, filteredObservations);
    }

    [Fact]
    public void Filter_ExcludesObservationsFromUnrelatedTaxa()
    {
        Taxon bird = new Taxon { TaxonId = 1, dwc_TaxonID = "bird" };
        Taxon fish = new Taxon { TaxonId = 2, dwc_TaxonID = "fish"};

        Observation observation = new Observation
        {
            Text = "A fish",
            Taxon = fish
        };

        TaxonFilterService filterService = new TaxonFilterService();
        List<Observation> filteredObservations = filterService.Filter(bird, new List<Observation> { observation });

        Assert.Empty(filteredObservations);
    }

    [Fact]
    public void Filter_HandlesEmptyObservationList()
    {
        Taxon bird = new Taxon { TaxonId = 1, dwc_TaxonID = "bird" };

        TaxonFilterService filterService = new TaxonFilterService();
        List<Observation> filteredObservations = filterService.Filter(bird, new List<Observation>());

        Assert.Empty(filteredObservations);
    }
}