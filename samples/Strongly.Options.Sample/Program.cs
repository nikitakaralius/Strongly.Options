using Microsoft.Extensions.Options;

using Strongly.Options;
using Strongly.Options.Sample.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSampleStronglyOptions(configuration);

var app = builder.Build();

app.MapGet("/record", (IOptions<ServiceOptions> options) => options.Value);

app.MapGet("/class", (IOptions<AuthOptions> options) => options.Value);

app.Run();
