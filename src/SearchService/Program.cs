using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;
using System;
using System.Net.Http;

namespace SearchService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await DbInitializer.InitDb(app);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            app.Run();

            static IAsyncPolicy<HttpResponseMessage> GetPolicy()
                => HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
        }
    }
}
