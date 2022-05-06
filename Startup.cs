using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Mini_Project.Controllers;
using Mini_Project.Data.Services;
using System.IO;

namespace Mini_Project
{
    public class Startup
    {
        private Utilities _utilities;

        public ParserController _parserController;
        public LoaderController _loaderController;
        public AggregatorController _aggregatorController;

        public ParserService _parserService;
        public LoaderService _loaderService;
        public AggregatorService _aggregatorService;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _utilities = new Utilities(configuration);

            _parserService = new ParserService(configuration);
            _loaderService = new LoaderService(configuration);
            _aggregatorService = new AggregatorService(configuration);

            _parserController = new ParserController(_parserService);
            _loaderController = new LoaderController(_loaderService);
            _aggregatorController = new AggregatorController(_aggregatorService);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mini_Project", Version = "v1" });
            });

            // Configure the Services
            services.AddTransient<LoaderService>();
            services.AddTransient<ParserService>();
            services.AddTransient<AggregatorService>();
            services.AddTransient<UIValuesService>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                services => services.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mini_Project v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Create a Watcher for Pasing Files.
            string parserPath = _utilities.GetParserFilesPath();

            FileSystemWatcher parserWatcher = new FileSystemWatcher();
            parserWatcher.Path = parserPath;

            parserWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            parserWatcher.Changed += ParserController.OnChanged;
            parserWatcher.Filter = _utilities.GetParserFilesExtensions();
            parserWatcher.EnableRaisingEvents = true;

            // Create a Watcher for Loading Files.
            string loaderPath = _utilities.GetLoaderFilesPath();

            FileSystemWatcher loaderWatcher = new FileSystemWatcher();
            loaderWatcher.Path = loaderPath;

            loaderWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            loaderWatcher.Changed += LoaderController.OnChanged;
            loaderWatcher.Filter = _utilities.GetLoaderFilesExtensions();
            loaderWatcher.EnableRaisingEvents = true;

            // Create a Watcher for Newly Loaded Files in Database.
            string aggregatorFromPath = _utilities.GetLoaderProcessedPath();

            FileSystemWatcher aggregatorWatcher = new FileSystemWatcher();
            aggregatorWatcher.Path = aggregatorFromPath;

            aggregatorWatcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size;

            aggregatorWatcher.Changed += AggregatorController.OnChanged;
            aggregatorWatcher.Filter = _utilities.GetLoaderFilesExtensions();
            aggregatorWatcher.EnableRaisingEvents = true;
        }
    }
}
