using Pingring.AddressNormalization.Api.Models;
using Pingring.AddressNormalization.Api.Services;

namespace Pingring.AddressNormalization.Api.Endpoints;

public sealed class NormalizationEndpoint
{
    public static async Task<IResult> Handler(
        NormalizationRequestPayload requestPayload,
        INormalizerService service,
        CancellationToken token
    )
    {
        var result = service.Normalize(
            requestPayload.Addresses,
            token
        );

        var response = new NormalizationResponsePayload
        {
            Addresses = await result.ToArrayAsync(token)
        };

        return Results.Ok(response);
    }
}

public sealed class NormalizationRequestPayload
{
    public required UnverifiedAddress[] Addresses { get; init; }
}

public sealed class NormalizationResponsePayload
{
    public required VerifiedAddress[] Addresses { get; init; }
}