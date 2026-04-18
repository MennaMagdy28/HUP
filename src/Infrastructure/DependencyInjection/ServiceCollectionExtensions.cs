using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleServicesAndRepositories(this IServiceCollection services, Assembly assembly)
        {
            var serviceTypes = assembly.GetTypes()
                .Where(t => (t.Name.EndsWith("Service") || t.Name.EndsWith("Repository"))
                            && t is { IsClass: true, IsAbstract: false });

            foreach (var type in serviceTypes)
            {
                var interfaceType = type.GetInterface($"I{type.Name}");

                if (interfaceType == null) continue;

                // You can expand logic here if needed (like singleton exceptions, e.g. CacheService)
                if (type.Name == "CacheService") continue;

                services.AddScoped(interfaceType, type);
            }

            return services;
        }
    }
}