using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Poke.Api.Integration;
using Poke.Api.Middleware;
using Poke.Api.Model;
using Poke.Api.Pipeline;
using Polly;
using Polly.Extensions.Http;

namespace Poke.Api
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
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Poke.Api", Version = "v1"}); });
            
            //Loosely coupled with the power of Mediatr Pipeline structure and dependency injection
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //Cache operation is registered to pipeline
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(CachePipelineBehaviour<,>));
            //Add FluentValidation to IOC container
            services.AddMvc()
                .AddNewtonsoftJson()
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });
            
            services.AddMemoryCache();
            //Add HttpClient for Poke API
            var pokeApiSettings = new PokeApiSettings();
            Configuration.GetSection("PokeApiSettings").Bind(pokeApiSettings);
            services.AddHttpClient<IPokeApiService, PokeApiService>(client =>
                {
                    client.BaseAddress = new Uri(pokeApiSettings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(3);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
            
            //Add HttpClient for Translation API
            var translationApiSettings = new TranslationApiSettings();
            Configuration.GetSection("TranslationApiSettings").Bind(translationApiSettings);
            services.AddHttpClient<ITranslationServiceApi, TranslationServiceApi>(client =>
                {
                    client.BaseAddress = new Uri(translationApiSettings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(3);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());
        }
        
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            //It is up to business and latency tolerance
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5,
                    retryAttempt)));
        }
        
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests
                                                                        || r.StatusCode == HttpStatusCode.ServiceUnavailable
                                                                        || r.StatusCode == HttpStatusCode.GatewayTimeout
                                                                        || r.StatusCode == HttpStatusCode.BadGateway)
                .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Poke.Api v1"));
            }
            
            //Middleware pipeline register, you can add more tasks to middleware, like logger etc
            app.UseMiddleware<ExceptionHandler>();
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}