using System;
using image_cloud_processor.MLModels;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using image_cloud_processor.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;

namespace image_cloud_processor
{
    public class Startup
    {
        readonly string AllowedOrigins = "_AllowedOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;


            // TODO: Put this path in appsettings
            var googleCredential = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(googleCredential))
            {
                Console.WriteLine("Google Credentials not SET - Loading for Dev Enviroment");
                string credential_path = @"C:\Users\a.de.melo.pinheiro\Documents\CESAR School\projeto-recopa\api-auth\API Project-64e82001381f.json";
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            }
            googleCredential = System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            Console.WriteLine("Google Credentials set:" + googleCredential);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowedOrigins,
                                  builder =>
                                  {
                                      builder
                                      .AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader();
                                      //WithOrigins("http://example.com", "http://www.contoso.com");
                                  });
            });

            services.AddSingleton<IDocumentosRepository<Document>, DocumentosRepository>();
            services.AddControllers();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            services.AddTransient<UploadService>();
            services.AddTransient<ImageService>();
            services.AddTransient<PredictionMLService>();

            services.AddTransient<DocumentService>();
            services.AddTransient<CloudImageProcessor>();

            services
            .AddPredictionEnginePool<ModelInput, ModelOutput>()
            //.FromFile(modelName: "", filePath: "")
            //.FromFile(modelName: "Field_SexoModel", filePath: "SexoMLModel.zip")
            //.FromFile(modelName: "Field_SintomaFebreModel", filePath: "SintomaFebreMLModel.zip");
            .FromUri(
                modelName: "Field_ResultadoTesteModel",
                uri: "https://github.com/projeto-recopa/recopa-machineleraning-models/raw/master/ResultadoTesteMLModel.zip",
                period: TimeSpan.FromMinutes(1))
            .FromUri(
                modelName: "Field_SexoModel",
                uri: "https://github.com/projeto-recopa/recopa-machineleraning-models/raw/master/SexoMLModel.zip",
                period: TimeSpan.FromMinutes(1))
            .FromUri(
                modelName: "Field_SintomaFebreModel",
                uri: "https://github.com/projeto-recopa/recopa-machineleraning-models/raw/master/SintomaFebreMLModel.zip",
                period: TimeSpan.FromMinutes(1));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Easy Typing Forms - API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowedOrigins);

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
