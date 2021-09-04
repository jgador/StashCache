using Microsoft.Extensions.DependencyInjection;

namespace StashCache
{
    /// <summary>
    /// Extension methods for setting up local in-memory services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class StashCacheExtensions
    {
        /// <summary>
        /// Adds services for local in-memory cache to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <returns>An Microsoft.Extensions.DependencyInjection.IServiceCollection.</returns>
        public static IServiceCollection AddStashCache(this IServiceCollection services)
        {
            services.NotNull(nameof(services));

            services.AddMemoryCache();

            services.AddSingleton<ILocalCache, LocalCache>();

            return services;
        }
    }
}
