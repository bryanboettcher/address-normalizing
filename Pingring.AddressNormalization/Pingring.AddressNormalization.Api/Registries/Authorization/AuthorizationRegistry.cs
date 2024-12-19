using Lamar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries;

public sealed class AuthorizationRegistry : ApplicationStartupRegistry
{
    /// <inheritdoc />
    public override ushort RunOrder => 100;

    public AuthorizationRegistry()
    {
        this.ConfigureOptions<ConfigureAuthorizationOptions>();
        this.AddAuthorization();
    }

    /// <inheritdoc />
    public override void Startup(IApplicationBuilder app)
    {
        app.UseAuthorization();
    }
}

public sealed class ConfigureAuthorizationOptions : IConfigureOptions<AuthorizationOptions>
{
    /// <inheritdoc />
    public void Configure(AuthorizationOptions options)
    {
    }
}
