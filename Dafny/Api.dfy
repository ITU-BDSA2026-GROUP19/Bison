include "Filter.dfy"

module Api {
    import opened FilterLib

    method FilterBy(root: Taxon, obs: seq<Observation>) returns (result: seq<Observation>)
    ensures (forall i:: 0 <= i < |result| ==> result[i].getTaxon().IsSubTaxon(root)) &&
    (forall i:: 0 <= i < |obs| ==> obs[i].getTaxon().IsSubTaxon(root) ==> obs[i] in result)
    {
        result := FilterLib.FilterBy(root, obs);
    }
}