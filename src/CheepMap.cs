using System.Data;
using CsvHelper.Configuration;

namespace Bison.Cheep;

public sealed class CheepMap : ClassMap<Cheep>
{
    public CheepMap() {
        //Map(m => m.Author).Name("Author");
        //Map(m => m.Message).Name("Observation");
        //Map(m => m.Timestamp).Name("Timestamp");
        Parameter("Author").Name("Author");
        Parameter("Message").Name("Observation");
        Parameter("Timestamp").Name("Timestamp");
    }
}