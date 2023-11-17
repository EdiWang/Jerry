# Jerry

Jerry, a static web server based on ASP.NET Core

## Install

You can choose either method to install Jerry

### .NET Tool

Requires [.NET 8.0 SDK](https://dot.net)

```powershell
dotnet tool install -g Jerry
```

### Docker

```
docker pull ediwang/jerry
```

## Usage


### As .NET Tool

```
Usage:
  Jerry [options]

Options:
  -d, --directory <directory>  Web content directory. [default: .]
  -p, --port <port>            HTTP server port. [default: 8080]
  --directory-browser          Directory browser [default: False]
  -v, --verbose                Verbose log [default: False]
  --version                    Show version information
  -?, -h, --help               Show help and usage information
```

Example

```powershell
jerry -d "E:\Workspace\wwwtest" --directory-browser
```

This will serve the content of `E:\Workspace\wwwtest` on port 8080, and enable directory browser.

### As Docker Container

```
docker run -p 8080:8080 -v /some/wonderful/wwwtest:/wwwtest ediwang/jerry -d /wwwtest
```