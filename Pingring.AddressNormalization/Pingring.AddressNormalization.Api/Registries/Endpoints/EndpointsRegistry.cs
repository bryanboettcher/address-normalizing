using Pingring.AddressNormalization.Api.Endpoints;
using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries.Endpoints;

public sealed class EndpointsRegistry : ApplicationStartupRegistry
{
    public EndpointsRegistry()
    {
    }

    /// <inheritdoc />
    public override ushort RunOrder => 50000;

    /// <inheritdoc />
    public override void Startup(IApplicationBuilder app)
    {
        if (app is not WebApplication webApp)
            return; // ... how?

        webApp.MapPost("/normalize", NormalizationEndpoint.Handler);
    }
}