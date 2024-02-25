using AuctionService.Data.DbContext;
using AuctionService.DTOs;
using AuctionService.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using MassTransit;
using AuctionService.Consumers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuctionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddScoped<IValidator<CreateAuctionDto>, CreateAuctionDtoValidator>();
            builder.Services.AddScoped<IValidator<UpdateAuctionDto>, UpdateAuctionDtoValidator>();
            builder.Services.AddDbContext<AuctionDbContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<AuctionDbContext>(opt =>
                {
                    opt.QueryDelay = TimeSpan.FromSeconds(10);
                    opt.UsePostgres();
                    opt.UseBusOutbox();
                });
                x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.Authority = builder.Configuration["IdentityServiceUrl"];
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters.ValidateAudience = false;
                    opt.TokenValidationParameters.NameClaimType = "userName";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            try
            {
                DbInitializer.InitDb(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            app.Run();
        }
    }
}
