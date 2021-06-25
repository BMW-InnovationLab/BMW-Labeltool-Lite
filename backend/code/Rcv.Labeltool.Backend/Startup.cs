using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rcv.LabelTool.Backend.Configuration;
using Rcv.LabelTool.Backend.Statics;
using Rcv.SwaggerExtensions;
using Serilog;
using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Backend
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        #region constants

        /// <summary>
        /// Name of CORS policy to use.
        /// </summary>
        private const string CORS_POLICY_NAME = "AllowAllOrigins";

        /// <summary>
        /// Application version.
        /// </summary>
        private const string APPLICATION_VERSION = "v1";

        /// <summary>
        /// Name of application.
        /// </summary>
        private const string APPLICATION_NAME = "RCV Labeltool Backend";

        #endregion

        #region member

        /// <summary>
        /// Configuration of application.
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of startup.
        /// </summary>
        /// <param name="configuration">Configuration of application</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion              

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services">Services</param>         
        public void ConfigureServices(IServiceCollection services)
        {
            BindOnPremiseFileContainer(services);
            services.BindFileSystemRepositories();

            services.BindConfigurations(Configuration);
            services.BindServices();

            // accessor to use httpcontext inside of methods
            services.AddHttpContextAccessor();
            services.BindHttpClients(Configuration);

            services.AddQueue();
            services.AddScheduler();

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add(CacheProfileNames.Default,
                    new CacheProfile()
                    {
                        Duration = 60
                    });
                options.CacheProfiles.Add(CacheProfileNames.Never,
                    new CacheProfile()
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSwaggerGen(c =>
            {
                c.ConfigureSwaggerGen(APPLICATION_VERSION, APPLICATION_NAME);
            });

            // add CORS rules
            services.AddCors(options =>
            {
                options.AddPolicy(CORS_POLICY_NAME, builder =>
                {
                    builder
                        .SetIsOriginAllowed(o => true) // allow all origins
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddSignalR();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Hosting environment</param>        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c =>
            {
                c.ConfigureSwagger();
            });
            app.UseSwaggerUI(c =>
            {
                c.ConfigureSwaggerUI(APPLICATION_NAME, APPLICATION_VERSION);
            });

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            Log.Information($"Start logging for {APPLICATION_NAME}");

            app.ApplicationServices.ConfigureQueue()
                .OnError(e =>
                {
                    Log.Error($"Error during execution of queued task. {e.Message}");
                });

            app.UseCors(CORS_POLICY_NAME);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region private helper

        private void BindOnPremiseFileContainer(IServiceCollection services)
        {
            OnPremiseConfiguration onPremiseConfiguration = Configuration.GetOnPremiseConfiguration();
            services.BindOnPremiseFileContainer(onPremiseConfiguration);
        }

        #endregion
    }
}
