# Strongly.Options

## About
Strongly.Options is a library that helps you to create strongly-typed options in AspNetCore with minimum effort.

## Use case

Register services
```csharp
var configuration = builder.Configuration;
builder.Services.AddStronglyOptions(configuration);
```

Define C# `class` or `record`
```csharp
[StronglyOptions("Service")]
public sealed record ServiceOptions
{
    public string Url { get; init; }

    public string Key { get; init; }
    
    public int RequestsPerHour { get; init; }
}
```
Add section inside configuration file <br />
`appsettings.json`
```json
{
  "Service": {
    "Url": "https://some-url-goes-here.com",
    "Key": "e324a183-54df-4f24-9db8-66322d066214",
    "RequestsPerSecond": 5
  }
}
```
Get Options as a dependency
```csharp
public sealed class Showcase 
{
    public Showcase(
        IOptions<ServiceOptions> options,
        IOptionsSnapshot<ServiceOptions> snapshot, // or
        IOptionsMonitor<ServiceOptions> monitor) // or
    {
        ...    
    }
}
```
