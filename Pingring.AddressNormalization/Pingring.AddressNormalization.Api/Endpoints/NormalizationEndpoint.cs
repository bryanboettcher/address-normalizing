namespace Pingring.AddressNormalization.Api.Endpoints;

public sealed class NormalizationEndpoint
{
    public static async Task Handler(
        NormalizationPayload payload,
        INormalizerService service,
        CancellationToken token
    )
    {
    }
}

public sealed class NormalizationPayload
{
    public required UnverifiedAddress[] Addresses { get; init; }
}

public sealed class UnverifiedAddress
{
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
}