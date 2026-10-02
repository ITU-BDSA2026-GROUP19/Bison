using System.Diagnostics;

namespace Bison.CLI.Tests;

public class E2ETests
{
    [Fact]
    public async Task Comment_NonExistingObservation_PrintsError()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli = TestHelpers.StartCli("comment", "999999", "Hello");

            string output = await cli.StandardOutput.ReadToEndAsync();
            await cli.WaitForExitAsync();

            Assert.Contains(
                "Observation with ID 999999 does not exist.",
                output);
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }

    [Fact]
    public async Task Read_PrintsObservations()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli = TestHelpers.StartCli("read");

            string output = await cli.StandardOutput.ReadToEndAsync();
            await cli.WaitForExitAsync();

            Assert.Contains("edka", output);
            Assert.Contains("Ardea cinerea at DR Byen", output);
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }

    [Fact]
    public async Task Observe_Adds_To_ObservationCsv()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli =
                TestHelpers.StartCli("observe", "Penguin", "Copenhagen");

            string filePath = "../../../../../data/bison_observations.csv";

            await cli.WaitForExitAsync();

            string contents = await File.ReadAllTextAsync(filePath);

            Assert.Contains("Penguin", contents);
            Assert.Contains("Copenhagen", contents);
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }

    [Fact]
    public async Task Location_Only_Prints_Observations_At_Location()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli = TestHelpers.StartCli("location", "Copenhagen");

            string output = await cli.StandardOutput.ReadToEndAsync();
            await cli.WaitForExitAsync();

            Assert.Contains("Copenhagen", output);
            Assert.DoesNotContain("Odense", output);
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }

    [Fact]
    public async Task Propose_ValidTaxon_Adds_To_ProposalCsv()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli = TestHelpers.StartCli(
                "propose",
                "3",
                "MSTSNM:Arter:c28811f4-f785-ea11-aa77-501ac539d1ea");

            await cli.WaitForExitAsync();

            string filePath = "../../../../../data/bison_proposals.csv";
            string contents = await File.ReadAllTextAsync(filePath);

            Assert.Contains(
                "MSTSNM:Arter:c28811f4-f785-ea11-aa77-501ac539d1ea",
                contents);
        }
        finally
        {
            if (!service.HasExited)
            {
                service.Kill(true);
            }
        }
    }

    [Fact]
    public async Task Propose_InvalidTaxon_DoesNotAdd_To_ProposalCsv()
    {
        Process service = TestHelpers.StartWebService();

        try
        {
            await TestHelpers.WaitForService();

            Process cli =
                TestHelpers.StartCli("propose", "3", "invalidTaxonId");

            await cli.WaitForExitAsync();

            string filePath = "../../../../../data/bison_proposals.csv";
            string contents = await File.ReadAllTextAsync(filePath);

            Assert.DoesNotContain("invalidTaxonId", contents);
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