using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Strongly.Options;

public static class StronglyOptionsExtensions
{
    private delegate IServiceCollection Configure(
        IServiceCollection services,
        IConfiguration configuration);

    public static IServiceCollection AddStronglyOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        var configureMethod = GetConfigureOptionsMethod();

        foreach (var (type, sectionName) in ScanStronglyConfigurationTypes(assembly))
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

    private static IEnumerable<(Type, string Section)> ScanStronglyConfigurationTypes(Assembly assembly) =>
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
