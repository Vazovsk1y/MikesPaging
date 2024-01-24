using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Services;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.Reflection;

namespace MikesPaging.AspNetCore;

public static class Registrator
{
    public static IServiceCollection AddPaging(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IPagingManager<>), typeof(DefaultPagingManager<>));
        return services;
    }

    public static IServiceCollection AddSorting(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(ISortingManager<>), typeof(DefaultSortingManager<>));
        return services;
    }

    public static IServiceCollection AddFiltering(this IServiceCollection services)
    {
        services.TryAddScoped(typeof(IFilteringManager<>), typeof(DefaultFilteringManager<>));
        return services;
    }

    public static IServiceCollection AddSortingConfigurationsFromAssembly(this IServiceCollection services, Assembly assembly) 
    {
        var configurationTypes = assembly
            .GetTypes()
            .Where(type =>
                !type.IsAbstract && type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == typeof(SortingConfiguration< , >))
            .ToList();

        foreach (var configurationType in configurationTypes)
        {
            var genericArguments = configurationType.BaseType!.GetGenericArguments();
            var interfaceType = typeof(ISortingConfiguration< , >).MakeGenericType(genericArguments);
            services.TryAddScoped(interfaceType, configurationType);
        }

        return services;
    }

    public static IServiceCollection AddFilteringConfigurationsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var configurationTypes = assembly
            .GetTypes()
            .Where(type =>
                !type.IsAbstract && type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == typeof(FilteringConfiguration< , >))
            .ToList();

        foreach (var filteringConfiguration in configurationTypes)
        {
            var genericArguments = filteringConfiguration.BaseType!.GetGenericArguments();
            var interfaceType = typeof(IFilteringConfiguration< , >).MakeGenericType(genericArguments);
            services.TryAddScoped(interfaceType, filteringConfiguration);
        }

        return services;
    }
}