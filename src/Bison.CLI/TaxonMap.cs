using CsvHelper.Configuration;

namespace Bison.Cheep;

public sealed class TaxonMap : ClassMap<Taxon>
{
    public TaxonMap() {
        
        //---- Mapping properties of the Taxon class to CSV columns ----
        Map(m => m.Id).Name("dwc:taxonID");
        Map(m => m.ParentId).Name("dwc:parentNameUsageID");
        Map(m => m.Rank).Name("dwc:taxonRank");
        Map(m => m.ScientificName).Name("dwc:scientificName");
        Map(m => m.VernacularName).Name("dwc:vernacularName");

        //---- Mapping parameters of the Taxon class to CSV columns using Parameter method ----
        Parameter("Id").Name("dwc:taxonID");
        Parameter("ParentId").Name("dwc:parentNameUsageID");
        Parameter("Rank").Name("dwc:taxonRank");
        Parameter("ScientificName").Name("dwc:scientificName");
        Parameter("VernacularName").Name("dwc:vernacularName");
    }
}