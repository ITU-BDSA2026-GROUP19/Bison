using System.Diagnostics;
using System.Net;

namespace Bison.Razor.Tests;

public sealed class RazorAppHelper : IDisposable
{
private readonly Process _process;
private readonly HttpClient _httpClient;
private bool _disposed;

public RazorAppHelper(string appPath, TestDatabase testDatabase)
{
    string baseUrl = "http://127.0.0.1:5274";

    var startInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    startInfo.ArgumentList.Add("run");
    startInfo.ArgumentList.Add("--project");
    startInfo.ArgumentList.Add(Path.Combine(appPath, "Bison.Razor.csproj"));
    startInfo.ArgumentList.Add("--no-launch-profile");
    startInfo.ArgumentList.Add("--urls");
    startInfo.ArgumentList.Add(baseUrl);

    startInfo.Environment["BISONDBPATH"] = testDatabase.DbPath;

    _process = new Process { StartInfo = startInfo };
    _process.Start();

    _httpClient = new HttpClient
    {
        BaseAddress = new Uri(baseUrl),
        Timeout = TimeSpan.FromSeconds(2)
    };

    try
    {
        WaitForServer();
    }
    catch
    {
        Dispose();
        throw;
    }
}

public async Task<string> GetAsync(string url)
{
    using var response = await _httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
}

private void WaitForServer()
{
    var stopwatch = Stopwatch.StartNew();

    while (stopwatch.ElapsedMilliseconds < 15000)
    {
        if (_process.HasExited)
        {
            string error = _process.StandardError.ReadToEnd();
            string output = _process.StandardOutput.ReadToEnd();

            throw new InvalidOperationException(
                $"Razor app exited with code {_process.ExitCode}.\n{output}\n{error}");
        }

        try
        {
            using var response = _httpClient.GetAsync("/obs/").GetAwaiter().GetResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
        }
        catch (HttpRequestException)
        {
            // The server may not be ready yet.
        }
        catch (TaskCanceledException)
        {
            // The readiness request timed out; try again.
        }

        Thread.Sleep(200);
    }

    throw new InvalidOperationException(
        "Razor app did not respond within 15 seconds.");
}

public void Dispose()
{
    if (_disposed)
    {
        return;
    }

    _disposed = true;

    try
    {
        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            _process.WaitForExit();
        }
    }
    finally
    {
        _process.Dispose();
        _httpClient.Dispose();
    }
}
}
