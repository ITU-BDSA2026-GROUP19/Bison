using System.Diagnostics;
using System.Net;

namespace Bison.CLI.Tests;

public class E2ETests
{
    [Fact]
    public async Task Comment_NonExistingObservation_PrintsError()
    {
        // Arrange
        Process service = StartWebService();

        try
        {
            await WaitForService();

            // Act
            Process cli = StartCli("comment", "999999", "Hello");

            string output = await cli.StandardOutput.ReadToEndAsync();
            await cli.WaitForExitAsync();

            // Assert
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

    static Process StartCli(string command, string argument1, string argument2)
    {
        string projectPath = FindProject("Bison.CLI");

        var process = new Process();

        process.StartInfo.FileName = "dotnet";
        process.StartInfo.ArgumentList.Add("run");
        process.StartInfo.ArgumentList.Add("--project");
        process.StartInfo.ArgumentList.Add(projectPath);
        process.StartInfo.ArgumentList.Add("--");
        process.StartInfo.ArgumentList.Add(command);
        process.StartInfo.ArgumentList.Add(argument1);
        process.StartInfo.ArgumentList.Add(argument2);

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
                // Service is not ready yet.
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