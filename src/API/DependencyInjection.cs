using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HUP.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var assemblies = entryAssembly?.GetReferencedAssemblies()
            .Where(a => a.FullName.StartsWith("HUP"))
            .Select(Assembly.Load)
            .ToList() ?? new List<Assembly>();
        
        if (entryAssembly != null) assemblies.Add(entryAssembly);

        var serviceTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                (t.Name.EndsWith("Service") || t.Name.EndsWith("Repository")));

        foreach (var implementation in serviceTypes)
        {
            var interfaces = implementation.GetInterfaces();

            foreach (var service in interfaces)
            {
                // Prevent duplicate registrations
                if (services.Any(d => d.ServiceType == service))
                    continue;

                services.AddScoped(service, implementation);
            }
        }
        
        return services;
    }
}