using Bison.Cheep;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Bison.CLI.Tests;

public class FuzzTests
{
    private List<Observation> expectedObservations = [];
    private List<Comment> expectedComments = [];
    private List<Proposal> expectedProposals = [];

    [Fact]
    public async Task Fuzz()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5273")
            };

            Random random = new();

            expectedObservations = await client.GetFromJsonAsync<List<Observation>>("/observations") ?? [];
            List<int> validObservationIds = expectedObservations.Select(observation => observation.Id).ToList();
            List<string> validTaxonIds = ["MSTSNM:Arter:3e4e67e4-f785-ea11-aa77-501ac539d1ea", 
            "MSTSNM:Arter:495067e4-f785-ea11-aa77-501ac539d1ea", "MSTSNM:Arter:a15367e4-f785-ea11-aa77-501ac539d1ea"];

            expectedComments = [];

            foreach (int observationId in validObservationIds)
            {
                List<Comment> comments = await client.GetFromJsonAsync<List<Comment>>($"/comments/{observationId}") ?? [];

                expectedComments.AddRange(comments);
            }



            for (int i = 0; i < 100; i++)
            {
                Observation observation = new(
                    expectedObservations.Select(o => o.Id).DefaultIfEmpty(0).Max() + 1,
                    $"user{random.Next(1000)}",
                    $"message{random.Next(1000)}",
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    $"location{random.Next(10)}"
                );

                HttpResponseMessage response =
                    await client.PostAsJsonAsync("/observation", observation);

                if (response.IsSuccessStatusCode)
                {
                    expectedObservations.Add(observation);
                    validObservationIds.Add(observation.Id);
                }
            }

            for (int i = 0; i < 100; i++)
            {
                int observationId =
                    validObservationIds[random.Next(validObservationIds.Count)];

                Comment comment = new(
                    observationId,
                    $"user{random.Next(1000)}",
                    $"message{random.Next(1000)}",
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                );

                HttpResponseMessage response =
                    await client.PostAsJsonAsync("/comment", comment);

                if (response.IsSuccessStatusCode)
                {
                    expectedComments.Add(comment);
                }
            }

            
            expectedProposals = [];

            for (int i = 0; i < 100; i++)
                {
                    int observationId =
                        validObservationIds[random.Next(validObservationIds.Count)];

                    string taxonId =
                        validTaxonIds[random.Next(validTaxonIds.Count)];

                    Proposal proposal = new(
                        observationId,
                        $"user{random.Next(1000)}",
                        taxonId,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    );

                    HttpResponseMessage response =
                        await client.PostAsJsonAsync("/proposal", proposal);

                    if (response.IsSuccessStatusCode)
                    {
                        expectedProposals.Add(proposal);
                    }
                }



                List<Observation> actualObservations = await client.GetFromJsonAsync<List<Observation>>("/observations") ?? [];

                Assert.Equal(expectedObservations, actualObservations);

                
                
                foreach (int observationId in validObservationIds)
                {
                    List<Comment> actualComments =
                        await client.GetFromJsonAsync<List<Comment>>($"/comments/{observationId}") ?? [];

                    List<Comment> expected =
                        expectedComments.Where(comment => comment.ObservationId == observationId).ToList();

                    Assert.Equal(expected, actualComments);
                }


                foreach (int observationId in validObservationIds)
                {
                    List<Proposal> actualProposals =
                        await client.GetFromJsonAsync<List<Proposal>>($"/proposals/{observationId}") ?? [];

                    List<Proposal> expected =
                        expectedProposals.Where(proposal => proposal.ObservationId == observationId).ToList();

                    Assert.Equal(expected, actualProposals);
                }
                
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }
}