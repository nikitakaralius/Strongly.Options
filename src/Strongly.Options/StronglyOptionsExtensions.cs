using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Strongly.Options;

public static class StronglyOptionsExtensions
{
    private delegate IServiceCollection Configure(
        IServiceCollection services,
        IConfiguration configuration);

    /// <summary>
    /// Scans the assembly of the specified type <typeparamref name="T"/> and registers all options types
    /// marked with the <see cref="StronglyOptionsAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type whose assembly should be scanned for options types.</typeparam>
    /// <example>
    /// <code>
    /// var configuration = builder.Configuration;
    /// builder.Services.AddStronglyOptions&lt;Program&gt;(configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddStronglyOptionsFromAssemblyContainingType<T>(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddStronglyOptions(configuration, typeof(T).Assembly);

    /// <summary>
    /// Scans the specified assembly and registers all options types marked with the <see cref="StronglyOptionsAttribute"/>.
    /// </summary>
    /// <param name="assembly">
    /// The assembly containing the options types. If not specified or null, uses the assembly of the calling method: <br />
    /// <c>Assembly.GetCallingAssembly()</c>
    /// </param>
    /// <example>
    /// <code>
    /// var configuration = builder.Configuration;
    /// builder.Services.AddStronglyOptions(configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddStronglyOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        var configureMethod = GetConfigureOptionsMethod();

        foreach (var (type, sectionName) in ScanStronglyOptionTypes(assembly))
        {
            var section = GetConfigurationSection(sectionName, configuration);
            var configure = BuildGenericConfigureMethod(type, configureMethod);
            configure(services, section);
        }

        return services;
    }

    private static MethodInfo GetConfigureOptionsMethod()
    {
        Type[] methodParameterTypes = [typeof(IServiceCollection), typeof(IConfiguration)];
        const string configureMethodName = nameof(OptionsConfigurationServiceCollectionExtensions.Configure);

        return typeof(OptionsConfigurationServiceCollectionExtensions)
           .GetMethod(configureMethodName, methodParameterTypes)!;
    }

    private static IEnumerable<(Type, string Section)> ScanStronglyOptionTypes(Assembly assembly) =>
        assembly
           .GetTypes()
           .Select(t => (t, t.GetCustomAttribute<StronglyOptionsAttribute>()?.Section))
           .Where(t => t.Section is not null)!;

    private static IConfiguration GetConfigurationSection(
        string section,
        IConfiguration configuration)
    {
        if (section == StronglyOptionsSection.Root)
            return configuration;

        return configuration.GetSection(section)
            ?? throw new SectionNotFoundException($"Unable to find {section} section inside appsettings.json");
    }

    private static Configure BuildGenericConfigureMethod(
        Type type,
        MethodInfo configureMethod)
    {
        const object? staticTarget = null;

        return (Configure) configureMethod
           .MakeGenericMethod(type)
           .CreateDelegate(typeof(Configure), staticTarget);
    }
}
