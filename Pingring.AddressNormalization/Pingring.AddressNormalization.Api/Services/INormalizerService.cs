using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Pingring.AddressNormalization.Api.Models;
using Pingring.AddressNormalization.Api.Registries.Application;

namespace Pingring.AddressNormalization.Api.Services;

public interface INormalizerService
{
    IAsyncEnumerable<VerifiedAddress> Normalize(IEnumerable<UnverifiedAddress> input, CancellationToken token);
}