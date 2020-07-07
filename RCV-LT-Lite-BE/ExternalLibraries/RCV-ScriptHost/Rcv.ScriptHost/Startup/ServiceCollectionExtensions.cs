using Microsoft.Extensions.DependencyInjection;
using Rcv.ScriptHost.Contracts.Container;
using Rcv.ScriptHost.Contracts.Models;
using Rcv.ScriptHost.Contracts.Services;
using Rcv.ScriptHost.Services.Containers;
using Rcv.ScriptHost.Services.Services;
using System;

namespace Rcv.ScriptHost
{
    /// <summary>
    /// Extensions for startup and registry of services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add configuration service to service context.
        /// </summary>
        /// <param name="serviceCollection">Service context</param>
        /// <param name="configuration">Configuration of host</param>
        public static void BindConfiguration(this IServiceCollection serviceCollection, ScriptHostConfiguration configuration)
        {
            #region validation

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            #endregion

            serviceCollection.AddTransient<ISettingsProvider, AppSettingsProvider>(o => new AppSettingsProvider(configuration));
        }

        /// <summary>
        /// Add all services to service context
        /// </summary>
        /// <param name="serviceCollection">Service context</param>
        public static void BindServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGuidService, GuidService>();
            serviceCollection.AddSingleton<IDateTimeService, DateTimeService>();

            serviceCollection.AddTransient<ISessionService, SessionService>();
            serviceCollection.AddTransient<IProcessHost, ProcessHost>();
        }

        /// <summary>
        /// Bind container management to service context.
        /// </summary>
        /// <param name="serviceCollection">Service context</param>
        public static void BindContainer(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFileContainerFactory, FileContainerFactory>();
        }
    }
}
