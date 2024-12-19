using Microsoft.Extensions.Options;
using Pingring.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pingring.AddressNormalization.Api.Registries.Swagger;

public sealed class SwaggerRegistry : ApplicationStartupRegistry
{
    /// <inheritdoc />
    public override ushort RunOrder => 1000;

    public SwaggerRegistry()
    {
        this.ConfigureOptions<ConfigureSwaggerGenOptions>();

        this.AddSwaggerGen();
        this.AddEndpointsApiExplorer();
    }

    /// <inheritdoc />
    public override void Startup(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}

public sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
    }
}