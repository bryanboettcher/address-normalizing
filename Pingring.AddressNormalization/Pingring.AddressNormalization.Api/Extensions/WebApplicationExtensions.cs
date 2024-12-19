using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Extensions;

public static class WebApplicationExtensions
{
    public static Task StartupAsync(this WebApplication app)
    {
        var services = app.Services;

        var startupPlugins = services
            .GetServices<ApplicationStartupRegistry>()
            .OrderBy(p => p.RunOrder)
            .ToList();

        foreach (var plugin in startupPlugins)
        {
            plugin.Startup(app);
        }

        return app.RunAsync();
    }
}
