
[![NuGet](https://img.shields.io/nuget/v/Ardalis.ApiClient.svg)](https://www.nuget.org/packages/Ardalis.ApiClient)[![NuGet](https://img.shields.io/nuget/dt/Ardalis.ApiClient.svg)](https://www.nuget.org/packages/Ardalis.ApiClient)

# Ardalis.ApiClient

Some classes to make working with APIs easier.  
For big example how to use ApiClient please visit [DevBetter](https://github.com/DevBetterCom/DevBetterWeb/tree/main/src/DevBetterWeb.Vimeo/Services/VideoServices)  


## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Credits

Big thanks to [all of the great contributors to this project](https://github.com/ardalis/Ardalis.ApiClient/graphs/contributors)!

## Getting Started

Install [Ardalis.ApiClient](https://nuget.org/Ardalis.ApiClient) from Nuget using:

(in Visual Studio)

```powershell
Install-Package Ardalis.ApiClient
```

(using the dotnet cli)

```powershell
dotnet add package Ardalis.ApiClient
```

In `Startup.cs` (or wherever you configure your services) add the following code. Change the base address to be the base URL where your APIs are hosted.

```csharp
public void ConfigureServices(IServiceCollection services)
{
  ....
  services.AddScoped(sp => HttpClientBuilder())
  services.AddScoped<HttpService>();
  services.AddScoped<AddVideoService>();
  ....
}

private static HttpClient HttpClientBuilder()
{
  var httpClient = new HttpClient
  {
    BaseAddress = new Uri("https://example.com")    
  };

  return httpClient;
}
```

Create a service file called `AddVideoService.cs` which is designed to call a particular API endpoint:

```csharp
public class AddVideoService : BaseAsyncApiCaller
  .WithRequest<VideoRequest>
  .WithResponse<VideoResponse>
{
  private readonly HttpService _httpService;
  private readonly ILogger<AddVideoService> _logger;

  public AddVideoService(HttpService httpService, ILogger<AddVideoService> logger)
  {
    _httpService = httpService;
    _logger = logger;
  }

  public override async Task<HttpResponse<VideoResponse>> ExecuteAsync(VideoRequest request,
    CancellationToken cancellationToken = default)
  {
    var uri = $"videos/add";
    try
    {
      var response = await _httpService.HttpPostAsync<VideoResponse>(uri, request);

      return response;
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
      return HttpResponse<VideoResponse>.FromException(exception.Message);
    }
  }
}
```

Call the service in Blazor:

```csharp
[Inject]
AddVideoService AddVideoService { get; set; }

private async Task<bool> AddVideoAsync()
{
  VideoRequest videoToAdd = new VideoRequest()
  {
    Title = Title,
    CreatedDate = CreatedDate
  };

  var result = await AddVideoService.ExecuteAsync(videoToAdd);
  if (result.Code != System.Net.HttpStatusCode.OK) return false;

  return result.Data;	
}
```
