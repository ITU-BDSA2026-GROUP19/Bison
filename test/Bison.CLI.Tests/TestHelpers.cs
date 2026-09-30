using System.Diagnostics;
using System.Net;

namespace Bison.CLI.Tests;

public static class TestHelpers
{
    public static Process StartWebService()
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

    public static async Task WaitForService()
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

    public static string FindProject(string projectName)
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


    public static Process StartCli(string command, params string[] arguments)
    {
        string projectPath = TestHelpers.FindProject("Bison.CLI");

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
}