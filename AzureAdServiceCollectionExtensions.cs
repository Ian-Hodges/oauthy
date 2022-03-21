using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using oauthy;
using System;

public static class AzureAdServiceCollectionExtensions
{
    public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder)
               => builder.AddAzureAdBearer(_ => { });

    public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
        builder.AddJwtBearer();
        return builder;
    }

    private class ConfigureAzureOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly AzureAdOptions _azureOptions;

        public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            options.TokenValidationParameters.ValidAudiences = new string[] { $"api://{_azureOptions.ClientId}", _azureOptions.ClientId };
            options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
            options.Events = JwtBearerMiddlewareDiagnostics.Subscribe(options.Events);
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}