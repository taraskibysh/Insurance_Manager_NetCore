namespace InsuranceCompany.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using InsuranceCompany.Application.Common.Interfaces.Authentication;
using InsuranceCompany.Infrastructure.Authentication;
using InsuranceCompany.Application.Common.Interfaces.Services;
using InsuranceCompany.Infrastructure.Services;
using InsuranceCompany.Application.Common.Interfaces.Persistance;
using InsuranceCompany.Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using InsuranceCompany.Application.Services.Business;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddAuth(configuration);
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPriceCalculator, PriceCalculator>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInsuranceRepository, InsuranceRepository>();
        services.AddScoped<IInsuranceService, InsuranceService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }

    public static IServiceCollection AddAuth(
    this IServiceCollection services,
    ConfigurationManager configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        // Register the JwtSettings instance
        services.AddSingleton(Options.Create(jwtSettings));

        // Add JWT Bearer Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)) 
                };

                // Add events to log token handling
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        if (context.Token != null)
                        {
                            logger.LogInformation("Received token: {Token}", context.Token);
                        }
                        else
                        {
                            logger.LogWarning("Token not present in the request.");
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogError("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogWarning("Authentication challenge failed: {ErrorDescription}", context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

}


       