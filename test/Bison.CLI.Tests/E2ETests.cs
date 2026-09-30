using System.Diagnostics;
using System.Net;

namespace Bison.CLI.Tests;

public class E2ETests
{
    [Fact]
    public async Task Comment_NonExistingObservation_PrintsError()
    {
        Process service = StartWebService();

        try
        {
            await WaitForService();

            Process cli = StartCli("comment", "999999", "Hello");

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
        Process service = StartWebService();

        try
        {
            await WaitForService();

            Process cli = StartCli("read");

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
        Process service = StartWebService();

        try
        {
            await WaitForService();

            Process cli = StartCli("observe", "Penguin", "Copenhagen");


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
        Process service = StartWebService();

        try
        {
            await WaitForService();

            Process cli = StartCli("location", "Copenhagen");


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
    Process service = StartWebService();

    try
    {
        await WaitForService();

        Process cli = StartCli("propose", "3", "MSTSNM:Arter:c28811f4-f785-ea11-aa77-501ac539d1ea");

        await cli.WaitForExitAsync();

        string filePath = "../../../../../data/bison_proposals.csv";
        string contents = await File.ReadAllTextAsync(filePath);

        Assert.Contains("MSTSNM:Arter:c28811f4-f785-ea11-aa77-501ac539d1ea", contents);
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
    Process service = StartWebService();

    try
    {
        await WaitForService();

        Process cli = StartCli("propose", "3", "invalidTaxonId");

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

    static Process StartWebService()
    {
        string projectPath = FindProject("Bison.CSVDBService");

        var process = new Process();

        process.StartInfo.FileName = "dotnet";
        process.StartInfo.ArgumentList.Add("run");
        process.StartInfo.ArgumentList.Add("--project");
        process.StartInfo.ArgumentList.Add(projectPath);

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;

        process.Start();

        return process;
    }

    static Process StartCli(string command, params string[] arguments)
    {
        string projectPath = FindProject("Bison.CLI");

        var process = new Process();

        process.StartInfo.FileName = "dotnet";
        process.StartInfo.ArgumentList.Add("run");
        process.StartInfo.ArgumentList.Add("--project");
        process.StartInfo.ArgumentList.Add(projectPath);
        process.StartInfo.ArgumentList.Add("--");
        process.StartInfo.ArgumentList.Add(command);

        foreach (string argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;

        process.Start();

        return process;
    }

    static async Task WaitForService()
    {
        using var client = new HttpClient();

        for (int i = 0; i < 50; i++)
        {
            try
            {
                HttpResponseMessage response =
                    await client.GetAsync("http://localhost:5273/observations");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch
            {
            }

            await Task.Delay(200);
        }

        throw new Exception("Web service did not start.");
    }

    static string FindProject(string projectName)
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            string projectPath =
                Path.Combine(
                    directory.FullName,
                    "src",
                    projectName,
                    $"{projectName}.csproj");

            if (File.Exists(projectPath))
            {
                return projectPath;
            }

            directory = directory.Parent;
        }

        throw new Exception($"Could not find {projectName}.csproj");
    }
}