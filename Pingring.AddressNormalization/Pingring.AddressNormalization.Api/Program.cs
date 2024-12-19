using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Pingring.AddressNormalization.Api.Extensions;
using Pingring.Common;

namespace Pingring.AddressNormalization.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseLamar(ConfigureContainer);

        var app = builder.Build();
        await app.StartupAsync();
    }

    private static void ConfigureContainer(
        HostBuilderContext context,
        ServiceRegistry services
    )
    {
        services.Scan(
            scan =>
            {
                scan.AssemblyContainingType<CommonScanAnchor>();
                scan.AssemblyContainingType<ProgramScanAnchor>();
                scan.LookForRegistries();
                scan.With(new AllInterfacesConvention());
            }
        );
    }
}