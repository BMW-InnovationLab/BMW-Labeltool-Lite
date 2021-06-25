using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rcv.LabelTool.Backend.Configuration;
using Rcv.LabelTool.Contracts.Configurations;
using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Services.DataStores;
using Rcv.LabelTool.Services.Repositories.File;
using Rcv.LabelTool.Services.Repositories.FileSystem;
using Rcv.LabelTool.Services.Services;
using Rcv.LabelTool.Web.Repositories;
using Rcv.LabelTool.Web.Services;
using System;
using System.Linq;
using System.Net.Http.Headers;

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

            services.BindSystemFileDataStore(EDataStoreType.TrainingData, onPremiseConfiguration.TrainingDataDirectoryPath);
        }

        /// <summary>
        /// Bind data store to file system path.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="dataStoreType">Type of data store</param>
        /// <param name="dataDirectoryPath">Path in filesystem</param>
        private static void BindSystemFileDataStore(this IServiceCollection services, EDataStoreType dataStoreType, string dataDirectoryPath)
        {
            if (!string.IsNullOrEmpty(dataDirectoryPath))
            {
                services.AddScoped<IDataStore, SystemFileDataStore>(o => new SystemFileDataStore(dataStoreType, dataDirectoryPath));
            }
        }

        /// <summary>
        /// Bind all repositories which are necessary for labeltool.
        /// </summary>
        /// <param name="services">Current context of services</param>
        public static void BindFileSystemRepositories(this IServiceCollection services)
        {
            services.AddTransient<IImageFileRepository, ImageFileRepository>(o => new ImageFileRepository(o.GetTrainingDataStore()));

            services.AddTransient<ITopicRepository, TopicRepository>(o => new TopicRepository(o.GetTrainingDataStore()));
            services.AddTransient<IObjectClassRepository, ObjectClassRepository>(o => new ObjectClassRepository(o.GetTrainingDataStore()));
            services.AddTransient<IImageDataRepository, ImageRepository>(o => new ImageRepository(o.GetTrainingDataStore(), o.GetRequiredService<ILabelRepository>()));
            services.AddTransient<IImageLabelNavigationRepository, ImageLabelNavigationFileSystemRepository>();
            services.AddTransient<ILabelRepository, LabelRepository>(o => new LabelRepository(o.GetTrainingDataStore()));
        }

        /// <summary>
        /// Bind all services which are necessary for labeltool.
        /// </summary>
        /// <param name="services">Current context of services</param>
        public static void BindServices(this IServiceCollection services)
        {
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IImageDataService, ImageDataService>();
            services.AddTransient<IImageUploadService, ImageUploadService>();
            services.AddTransient<ILabelService, LabelService>();
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<IObjectClassService, ObjectClassService>();
            services.AddTransient<ISuggestService, SuggestService>();

            // image services
            services.AddTransient<IImageNavigationService, ImageNavigationService>();
        }

        /// <summary>
        /// Bind configurations instances.
        /// </summary>
        /// <param name="services">Current context of services</param>
        /// <param name="configuration">Configuration of application</param>
        public static void BindConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            #region validation

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            #endregion

            services.AddSingleton<IInferenceConfiguration, InferenceConfiguration>(o => configuration.GetSegmentationConfiguration());
        }

        /// <summary>
        /// Bind http clients to service instances.
        /// </summary>
        /// <param name="services">Current context of services</param>
        /// <param name="configuration">Configuration of application</param>
        public static void BindHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(EHttpClients.InferenceContainerClient.ToString(), client =>
            {
                client.BaseAddress = new System.Uri(configuration.GetSegmentationConfiguration().InferenceContainerUri);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
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
