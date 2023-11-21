# Jerry

Jerry, a static web server based on ASP.NET Core

## Install

You can choose either method to install Jerry

### Compiled Binaries (AOT)

Download executable for your OS platform in [Release](https://github.com/EdiWang/Jerry/releases), without installing .NET runtime.

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


### As .NET Tool or Standalone Executable

```
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

![image](https://github.com/EdiWang/Jerry/assets/3304703/e0402373-a897-4170-9d5d-bdc6caf75d03)

### As Docker Container

```
docker run -p 8080:8080 -v /some/wonderful/wwwtest:/wwwtest ediwang/jerry -d /wwwtest
```

## 免责申明

Jerry 及其配套组件均为免费开源的产品，仅用于学习交流，并且不直接向中国提供服务，中国用户请于下载后立即删除。

任何中国境内的组织及个人不得使用 Jerry 及其配套组件构建任何形式的面向中国境内用户的网站或服务。

不可用于任何违反中华人民共和国(含台湾省)或使用者所在地区法律法规的用途。

因为作者即本人仅完成代码的开发和开源活动(开源即任何人都可以下载使用)，从未参与用户的任何运营和盈利活动。

且不知晓用户后续将程序源代码用于何种用途，故用户使用过程中所带来的任何法律责任即由用户自己承担。

[《开源软件有漏洞，作者需要负责吗？是的！》](https://go.edi.wang/aka/os251)