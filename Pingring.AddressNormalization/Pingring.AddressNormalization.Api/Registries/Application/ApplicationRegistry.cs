using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Registries.Application;

public sealed class ApplicationRegistry : ApplicationStartupRegistry
{
    public ApplicationRegistry()
    {
    }

    public override ushort RunOrder => 10000;
}