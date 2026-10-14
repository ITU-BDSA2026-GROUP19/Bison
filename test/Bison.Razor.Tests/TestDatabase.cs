using Microsoft.EntityFrameworkCore;

namespace Bison.Razor.Tests;

public class TestDatabase : IDisposable
{
    public string DbPath { get; }

    public TestDatabase()
    {
        DbPath = Path.Combine(
            Path.GetTempPath(),
            $"bison-test-{Guid.NewGuid()}.db");

        var DbOptions = new DbContextOptionsBuilder<BisonDBContext>()
        .UseSqlite($"Data Source={DbPath}").Options;

        using(var context = new BisonDBContext(DbOptions))
        {
            context.Database.Migrate();
            context.Authors.AddRange(new Author { Name = "Peter", Email = "peter@example.com" },
                                 new Author { Name = "Petra", Email = "petra@example.com" },
                                 new Author { Name = "Paul", Email = "paul@example.com" });
            context.SaveChanges();

            Taxon taxon = new Taxon { dwc_TaxonID = "123", TaxonId = 1, VernacularName = "Heron" };
            context.Taxa.Add(taxon);
            context.SaveChanges();

            context.Observations.AddRange(
                new Observation
                {
                    Text = "A big gray bird in a pond at DR byen",
                    TimeStamp = DateTime.Now,
                    Author = context.Authors.First(a => a.Name == "Peter"),
                    Taxon = taxon
                },
                new Observation
                {
                    Text = "A heron",
                    TimeStamp = DateTime.Now,
                    Author = context.Authors.First(a => a.Name == "Petra"),
                    Taxon = taxon
                });
            context.SaveChanges();
        }
    }

    public void Dispose()
    {
        if (File.Exists(DbPath))
        {
            File.Delete(DbPath);
        }
    }
}