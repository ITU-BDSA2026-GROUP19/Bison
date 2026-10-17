
using Bison.Model;

public interface ITaxonFilterService
{
    List<Observation> Filter(Taxon root, List<Observation> obs);
}

public class TaxonFilterService : ITaxonFilterService
{
    public List<Observation> Filter(Taxon root, List<Observation> obs)
    {
        Observation[] obsArray = obs.ToArray();
        Dafny.ISequence<Observation> seqOfObs = Dafny.Sequence<Observation>.FromElements(obsArray);
        List<Observation> filteredList = Api.__default.FilterBy(root, seqOfObs).ToList();
        return filteredList;
    }
}
