using Microsoft.Extensions.FileProviders;
using System.CommandLine;

namespace Jerry;

public class Program
{
    public static async Task Main(string[] args)
    {
        var rootCommand = BuildCommand();

        try
        {
            await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex.Message}");
            Environment.Exit(1);
        }
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

        var httpsOption = new Option<bool>(
            "--use-https",
            () => false,
            "Enable HTTPS");

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
        rootCommand.AddOption(httpsOption);
        rootCommand.AddOption(browserOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(async (path, port, https, directoryBrowser, verbose) =>
        {
            var host = BuildApp(path, port, https, directoryBrowser, verbose);
            LogInformation(host, $"Serving directory: '{path}'");
            await host.RunAsync();
        }, directoryOption, portOption, httpsOption, browserOption, verboseOption);

        return rootCommand;
    }

    private static WebApplication BuildApp(string path, int port, bool https, bool directoryBrowser, bool verbose)
    {
        var webRoot = Path.GetFullPath(path);
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            WebRootPath = webRoot
        });

        builder.Logging.SetMinimumLevel(verbose ? LogLevel.Trace : LogLevel.Information);

        if (directoryBrowser) builder.Services.AddDirectoryBrowser();

        builder.Services.AddResponseCompression();

        ConfigureKestrel(builder.WebHost, port, https);

        var host = builder.Build();

        ConfigureMiddleware(host, directoryBrowser);

        return host;
    }

    private static void ConfigureKestrel(IWebHostBuilder webHostBuilder, int port, bool https)
    {
        webHostBuilder.ConfigureKestrel(options =>
        {
            if (https)
            {
                options.ListenAnyIP(port, listenOptions => listenOptions.UseHttps());
            }
            else
            {
                options.ListenAnyIP(port);
            }

            options.AddServerHeader = false;
        });
    }

    private static void ConfigureMiddleware(WebApplication host, bool directoryBrowser)
    {
        host.UseResponseCompression();
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