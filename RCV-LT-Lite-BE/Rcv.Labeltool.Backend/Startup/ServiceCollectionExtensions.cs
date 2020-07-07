using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rcv.LabelTool.Backend.Configuration;
using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Services.DataStores;
using Rcv.LabelTool.Web.Repositories;
using Rcv.LabelTool.Web.Services;
using System;
using System.Linq;

namespace Rcv.LabelTool.Backend
{
    /// <summary>
    /// Extension methods for service context.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Bind all filecontainer which are located to traingingsdata-path.
        /// </summary>
        /// <param name="services">Current context of services</param>
        /// <param name="onPremiseConfiguration">Configuration for on premise file containers</param>
        public static void BindOnPremiseFileContainer(this IServiceCollection services, OnPremiseConfiguration onPremiseConfiguration)
        {
            #region validation

            if (onPremiseConfiguration == null)
            {
                throw new ArgumentNullException(nameof(onPremiseConfiguration));
            }

            #endregion

            services.AddTransient<IDataStore, SystemFileDataStore>(o => new SystemFileDataStore(EDataStoreType.TrainingData, onPremiseConfiguration.TrainingDataDirectoryPath));
        }

        /// <summary>
        /// Bind all repositories which are necessary for labeltool.
        /// </summary>
        /// <param name="services">Current context of services</param>
        public static void BindRepositories(this IServiceCollection services)
        {
            services.AddTransient<ITopicRepository, TopicRepository>(o => new TopicRepository(o.GetTrainingDataStore()));
            services.AddTransient<IObjectClassRepository, ObjectClassRepository>(o => new ObjectClassRepository(o.GetTrainingDataStore()));
            services.AddTransient<IImageRepository, ImageRepository>(o => new ImageRepository(o.GetTrainingDataStore(), o.GetRequiredService<IHttpContextAccessor>()));
            services.AddTransient<ILabelRepository, LabelRepository>(o => new LabelRepository(o.GetTrainingDataStore()));
        }

        /// <summary>
        /// Bind all services which are necessary for labeltool.
        /// </summary>
        /// <param name="services">Current context of services</param>
        public static void BindServices(this IServiceCollection services)
        {
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ILabelService, LabelService>();
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<IObjectClassService, ObjectClassService>();
        }

        #region private helper

        /// <summary>
        /// Get training datastore from service provider.
        /// </summary>
        /// <param name="serviceProvider">Current context of services</param>
        /// <returns>Training datastore</returns>
        private static IDataStore GetTrainingDataStore(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetDataStore(EDataStoreType.TrainingData);
        }

        /// <summary>
        /// Get datastore by type from service provider.
        /// </summary>
        /// <param name="serviceProvider">Current context of services</param>
        /// <param name="dataStoreType">Type of datastore</param>
        /// <returns>Datastore of specified type</returns>
        private static IDataStore GetDataStore(this IServiceProvider serviceProvider, EDataStoreType dataStoreType)
        {
            return serviceProvider.GetServices<IDataStore>().Single(s => s.DataStoreType == dataStoreType);
        }

        #endregion
    }
}
