# Strongly.Options

## About
Strongly.Options is a library that helps you to create strongly-typed options in AspNetCore with minimum effort.

## Use case

Register services
```csharp
// Assembly: CoolApi
var configuration = builder.Configuration;

// AddCoolApiStronglyOptions will be created by a Source Generator
builder.Services.AddCoolApiStronglyOptions(configuration);
```

Define C# `class` or `record`
```csharp
using Strongly.Options;

[StronglyOptions("Service")]
public sealed record ServiceOptions
{
    public string Url { get; init; }

    public Guid Key { get; init; }
    
    public int RequestsPerHour { get; init; }
}
```
Add section inside configuration (e.g., `appsettings.json`)
```json
{
  "Service": {
    "Url": "https://some-url-goes-here.com",
    "Key": "e324a183-54df-4f24-9db8-66322d066214",
    "RequestsPerHour": 5
  }
}
```
Get Options as a dependency
```csharp
using Strongly.Options;

public sealed class Showcase 
{
    public Showcase(
        IOptions<ServiceOptions> options,
        IOptionsSnapshot<ServiceOptions> snapshot, // or
        IOptionsMonitor<ServiceOptions> monitor)   // or
    {
        ...    
    }
}
```

## Sample Projects

To see this in practice you can examine [Sample Projects](samples)
