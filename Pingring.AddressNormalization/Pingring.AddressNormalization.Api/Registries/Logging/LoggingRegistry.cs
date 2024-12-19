using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries.Logging;

public sealed class LoggingRegistry : ApplicationStartupRegistry
{
    public LoggingRegistry()
    {
        this.AddLogging();

        Policies.Add<LoggerPolicy>();
    }

    public override ushort RunOrder => 10000;
}
