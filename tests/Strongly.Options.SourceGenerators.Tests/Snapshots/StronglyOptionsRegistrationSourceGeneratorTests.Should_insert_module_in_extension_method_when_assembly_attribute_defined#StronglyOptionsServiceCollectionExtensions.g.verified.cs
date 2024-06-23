//HintName: StronglyOptionsServiceCollectionExtensions.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Strongly.Options
{
    public static class StronglyOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// Scans current assembly and registers all options types marked with the <see cref="StronglyOptionsAttribute"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// var configuration = builder.Configuration;
        /// builder.Services.AddTestsStronglyOptions(configuration);
        /// </code>
        /// </example>
        public static IServiceCollection AddTestsStronglyOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<global::Company.Application.AuthOptions>(configuration.GetSection("Auth") ?? throw new SectionNotFoundException($"Unable to find \"Auth\" section in IConfiguration"));
    
            return services;
        }
    }
}
