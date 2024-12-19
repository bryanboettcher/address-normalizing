using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries.Hosting;

public sealed class HostingRegistry : ApplicationStartupRegistry
{
    /// <inheritdoc />
    public override ushort RunOrder => 2000;

    public HostingRegistry()
    {
    }

    /// <inheritdoc />
    public override void Startup(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
    }
}