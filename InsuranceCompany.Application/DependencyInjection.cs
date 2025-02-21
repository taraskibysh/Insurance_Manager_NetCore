namespace InsuranceCompany.Application;
using InsuranceCompany.Application.Services.Authentication;
using InsuranceCompany.Application.Services.Business;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IInsuranceService, InsuranceService>();


        return services;
    }
}