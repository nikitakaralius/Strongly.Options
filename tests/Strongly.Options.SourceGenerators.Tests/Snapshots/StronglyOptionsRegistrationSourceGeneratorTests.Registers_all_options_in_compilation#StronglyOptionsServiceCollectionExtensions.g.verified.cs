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
        /// builder.Services.AddStronglyOptions(configuration);
        /// </code>
        /// </example>
        public static IServiceCollection AddStronglyOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<global::Company.Application.AuthOptions>(configuration.GetRequiredSection("Auth"));
            services.Configure<global::ServiceOptions>(configuration.GetRequiredSection("Service"));
    
            return services;
        }
    }
}
