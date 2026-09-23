using CsvHelper.Configuration;

namespace Bison.Cheep;

public sealed class ObservationMap : ClassMap<Observation>
{
    public ObservationMap() {
        
        //---- Mapping properties of the Observation class to CSV columns ----
        Map(m => m.Id).Name("Id");
        Map(m => m.Author).Name("Author");
        Map(m => m.Message).Name("Message");
        Map(m => m.Timestamp).Name("Timestamp");
        Map(m => m.Location).Name("Location");

        //---- Mapping parameters of the Observation class to CSV columns using Parameter method ----
        Parameter("Id").Name("Id");
        Parameter("Author").Name("Author");
        Parameter("Message").Name("Message");
        Parameter("Timestamp").Name("Timestamp");
        Parameter("Location").Name("Location");
    }
}