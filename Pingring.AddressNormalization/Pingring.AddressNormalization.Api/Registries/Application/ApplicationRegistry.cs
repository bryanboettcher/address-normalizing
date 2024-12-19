using Microsoft.Extensions.Options;
using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries.Application;

public sealed class ApplicationRegistry : ApplicationStartupRegistry
{
    public ApplicationRegistry()
    {
        this.ConfigureOptions<ConfigureLobNormalizerOptions>();
    }

    public override ushort RunOrder => 10000;
}

public sealed class ConfigureLobNormalizerOptions : IConfigureOptions<LobNormalizerOptions>
{
    private readonly IConfiguration _config;

    public ConfigureLobNormalizerOptions(IConfiguration config) => _config = config;

    public void Configure(LobNormalizerOptions options)
    {
        options.ApiKey = _config["LOB:ApiKey"];
        options.BaseUrl = _config["LOB:BaseUrl"];
    }
}

public sealed class LobNormalizerOptions
{
    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
}