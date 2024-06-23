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
            services.Configure<global::Company.Application.AuthOptions>(GetConfigurationSection("Auth", configuration));
    
            return services;
        }
        
        
        private static IConfiguration GetConfigurationSection(string sectionName, IConfiguration configuration)
        {
            if (sectionName == StronglyOptionsSection.Root)
                return configuration;
        
            var section = configuration.GetSection(sectionName);
        
            if (!section.AsEnumerable().Any(x => x.Value is not null))
                throw new SectionNotFoundException(
                    $"Unable to find {section} section in Configuration");
        
            return section;
        }
    }
}
