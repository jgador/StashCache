using Microsoft.Extensions.DependencyInjection;

namespace StashCache
{
    public static class StashCacheExtensions
    {
        public static IServiceCollection AddStashCache(this IServiceCollection services)
        {
            services.NotNull(nameof(services));

            services.AddMemoryCache();

            services.AddSingleton<ILocalCache, LocalCache>();

            return services;
        }
    }
}
