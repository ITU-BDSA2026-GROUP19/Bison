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