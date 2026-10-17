module {:extern "Bison.Model"} FilterLib {
class {:extern "Taxon"} Taxon {
function {:extern} IsSubTaxon(ancestor: Taxon): bool
}
class {:extern "Observation"} Observation {
function {:extern} getTaxon(): Taxon
}
function FilterBy(root: Taxon, obs: seq<Observation>): (result: seq<Observation>)
ensures (forall i:: 0 <= i < |result| ==> result[i].getTaxon().IsSubTaxon(root)) &&
(forall i:: 0 <= i < |obs| ==> obs[i].getTaxon().IsSubTaxon(root) ==> obs[i] in result)
{
    if |obs| == 0 then []
    else 
        if obs[0].getTaxon().IsSubTaxon(root) then obs[0..1] + FilterBy(root, obs[1..])
        else FilterBy(root, obs[1..])
}
}