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

    public static RootCommand BuildCommand()
    {
        var directoryOption = new Option<string>(
            aliases: new[] { "--directory", "-d" },
            getDefaultValue: () => ".",
            description: "Web content directory.");

        var portOption = new Option<int>(
            aliases: new[] { "--port", "-p" },
            getDefaultValue: () => 8080,
            description: "HTTP server port.");

        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            getDefaultValue: () => false,
            description: "Verbose log");

        var browserOption = new Option<bool>(
            name: "--directory-browser",
            getDefaultValue: () => false,
            description: "Directory browser");

        var rootCommand = new RootCommand("Jerry, a static web server based on ASP.NET Core.");
        rootCommand.AddOption(directoryOption);
        rootCommand.AddOption(portOption);
        rootCommand.AddOption(browserOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(async (path, port, directoryBrowser, verbose) =>
        {
            var host = BuildApp(path, port, directoryBrowser, verbose);
            host.Logger.LogInformation($"Serving directory: '{path}'");

            await host.RunAsync();
        }, directoryOption, portOption, browserOption, verboseOption);

        return rootCommand;
    }

    public static WebApplication BuildApp(string path, int port, bool directoryBrowser, bool verbose)
    {
        var webRoot = Path.GetFullPath(path);
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            WebRootPath = webRoot
        });

        builder.Logging.SetMinimumLevel(verbose ? LogLevel.Trace : LogLevel.Information);

        if (directoryBrowser) builder.Services.AddDirectoryBrowser();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(port);
            options.AddServerHeader = false;
        });

        var host = builder.Build();

        host.UseDefaultFiles();

        host.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true
        });

        if (directoryBrowser)
        {
            var fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath));
            host.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = fileProvider
            });
        }

        return host;
    }
}