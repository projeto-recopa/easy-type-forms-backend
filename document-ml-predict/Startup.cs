using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using image_cloud_processor.MLModels;
using image_cloud_processor.Models;
using image_cloud_processor.Repository;
using image_cloud_processor.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;

namespace document_ml_predict
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IDocumentosRepository<Document>, DocumentosRepository>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();


            services.AddTransient<PredictionMLService>();

            services
           .AddPredictionEnginePool<ModelInput, ModelOutput>()
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
               period: TimeSpan.FromMinutes(1))
           ;
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            //services.AddApplicationInsightsTelemetry(Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY"));
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
