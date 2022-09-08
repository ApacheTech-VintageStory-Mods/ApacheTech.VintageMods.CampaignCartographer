using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Vintagestory.API.Common;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions
{
    /// <summary>
    ///     Extension methods to add hosting proprietary mod systems.
    /// </summary>
    public static class ModSystemHostExtensions
    {
        /// <summary>
        ///     Adds a proprietary mod system, from the vanilla game, to the IOC container.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        public static void AddProprietaryModSystem<TService>(this IServiceCollection services)
            where TService : ModSystem
        {
            services.AddSingleton(ioc => ioc.Resolve<ICoreAPI>().ModLoader.GetModSystem<TService>());
        }

        /// <summary>
        ///     Adds a proprietary mod system, from the vanilla game, to the IOC container.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        public static void AddProprietaryModSystem<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : ModSystem, TService
        {
            services.AddSingleton<TService>(ioc => ioc.Resolve<ICoreAPI>().ModLoader.GetModSystem<TImplementation>());
        }
    }
}