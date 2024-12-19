using System.Collections.Concurrent;
using System.Reflection;
using JasperFx.Core.TypeScanning;
using Lamar;
using Lamar.IoC.Instances;
using Lamar.Scanning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Pingring.Common;

public sealed class AllInterfacesConvention : IRegistrationConvention
{
    public void ScanTypes(TypeSet types, ServiceRegistry services)
    {
        MatchClosedTypes(types, services);
        MatchOpenTypes(types, services);
    }
    private static void MatchOpenTypes(TypeSet types, ServiceRegistry services)
    {
        var matchedTypes = types.FindTypes(TypeClassification.Concretes | TypeClassification.Open);

        foreach (var type in matchedTypes)
        {
            if (type.GetCustomAttribute<ExcludeFromAutoScanAttribute>() is not null)
                continue;

            // Register against all the interfaces implemented
            // by this concrete class
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType)
                    continue;

                if (iface.GetCustomAttribute<ExcludeFromAutoScanAttribute>() is not null)
                    continue;

                services.For(iface).Use(type);
            }
        }
    }

    private static void MatchClosedTypes(TypeSet types, ServiceRegistry services)
    {
        var matchedTypes = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed);

        // Only work on concrete types
        foreach (var type in matchedTypes)
        {
            if (type.GetCustomAttribute<ExcludeFromAutoScanAttribute>() is not null)
                continue;

            // Register against all the interfaces implemented
            // by this concrete class
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.GetCustomAttribute<ExcludeFromAutoScanAttribute>() is not null)
                    continue;

                services.For(iface).Use(type);
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class ExcludeFromAutoScanAttribute : Attribute
{
}

public readonly struct CommonScanAnchor { }

public sealed class LoggerPolicy : ConfiguredInstancePolicy
{
    private static readonly ConcurrentDictionary<Type, ILogger> LoggerTypeCache = new();
    private static readonly Type GenericLoggerInterface = typeof(ILogger<>);

    /// <inheritdoc />
    protected override void apply(IConfiguredInstance instance)
    {
        var type = instance.ImplementationType;

        var parameters = type.GetConstructors()
                             .SelectMany(x => x.GetParameters())
                             .Where(x => x.ParameterType == typeof(ILogger));

        foreach (var param in parameters)
        {
            instance.Ctor<ILogger>(param.Name).Is(ConfigureParameter);
        }

        return;

        ILogger ConfigureParameter(IServiceContext ctx) => LoggerTypeCache.GetOrAdd(type, _ => CreateLogger(ctx));

        ILogger CreateLogger(IServiceContext ctx) => (ILogger) ctx.GetInstance(GenericLoggerInterface.MakeGenericType(type));
    }
}

public abstract class ApplicationStartupRegistry : ServiceRegistry
{
    /// <summary>
    /// Controls when the second phase of startup code runs.  Smaller numbers run earlier.
    ///
    /// There are 65,536 slots available, don't bunch everything up from 1-10.  Registries
    /// with duplicate numbers will run sequentially sorted alphabetically.
    /// </summary>
    public abstract ushort RunOrder { get; }

    public virtual void Startup(IApplicationBuilder app) { }
}