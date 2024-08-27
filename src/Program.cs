using Microsoft.Extensions.FileProviders;
using System.CommandLine;

namespace Jerry;

public class Program
{
    public static async Task Main(string[] args)
    {
        var rootCommand = BuildCommand();
        await rootCommand.InvokeAsync(args);
    }

    private static RootCommand BuildCommand()
    {
        var directoryOption = new Option<string>(
            ["--directory", "-d"],
            () => ".",
            "Web content directory.");

        var portOption = new Option<int>(
            ["--port", "-p"],
            () => 8080,
            "HTTP server port.");

        var verboseOption = new Option<bool>(
            ["--verbose", "-v"],
            () => false,
            "Verbose log");

        var browserOption = new Option<bool>(
            "--directory-browser",
            () => false,
            "Directory browser");

        var rootCommand = new RootCommand("Jerry, a static web server based on ASP.NET Core.");
        rootCommand.AddOption(directoryOption);
        rootCommand.AddOption(portOption);
        rootCommand.AddOption(browserOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(async (path, port, directoryBrowser, verbose) =>
        {
            var host = BuildApp(path, port, directoryBrowser, verbose);
            LogInformation(host, $"Serving directory: '{path}'");
            await host.RunAsync();
        }, directoryOption, portOption, browserOption, verboseOption);

        return rootCommand;
    }

    private static WebApplication BuildApp(string path, int port, bool directoryBrowser, bool verbose)
    {
        var webRoot = Path.GetFullPath(path);
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            WebRootPath = webRoot
        });

        builder.Logging.SetMinimumLevel(verbose ? LogLevel.Trace : LogLevel.Information);

        if (directoryBrowser) builder.Services.AddDirectoryBrowser();

        ConfigureKestrel(builder.WebHost, port);

        var host = builder.Build();

        ConfigureMiddleware(host, directoryBrowser);

        return host;
    }

    private static void ConfigureKestrel(IWebHostBuilder webHostBuilder, int port)
    {
        webHostBuilder.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(port);
            options.AddServerHeader = false;
        });
    }

    private static void ConfigureMiddleware(WebApplication host, bool directoryBrowser)
    {
        host.UseDefaultFiles();
        host.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true
        });

        if (directoryBrowser)
        {
            var fileProvider = new PhysicalFileProvider(host.Environment.WebRootPath);
            host.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider
            });
        }
    }

    private static void LogInformation(WebApplication host, string message)
    {
        host.Logger.LogInformation(message);
    }
}