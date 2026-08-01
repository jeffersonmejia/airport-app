using Airport.Features.Payments.Application.Ports;
using Airport.Features.Payments.Infrastructure.PayPal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airport.Features.Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<PayPalOptions>()
            .Bind(configuration.GetRequiredSection(PayPalOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ClientId),
                "PayPal ClientId is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ClientSecret),
                "PayPal ClientSecret is required.")
            .Validate(options => options.HasValidSandboxBaseUrl,
                "PayPal BaseUrl must target the HTTPS sandbox API.")
            .Validate(options => options.HasValidWebUrls,
                "PayPal ReturnUrl and CancelUrl must be absolute URLs.")
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddHttpClient(PayPalOptions.HttpClientName, (provider, client) =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<PayPalOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddSingleton<PayPalAccessTokenProvider>();
        services.AddScoped<IPayPalGateway, PayPalPaymentGateway>();

        return services;
    }
}
