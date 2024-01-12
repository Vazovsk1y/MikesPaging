using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikesPaging.Core.Services;
using MikesPaging.Core.Services.Interfaces;

namespace MikesPaging.Core;

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
}
