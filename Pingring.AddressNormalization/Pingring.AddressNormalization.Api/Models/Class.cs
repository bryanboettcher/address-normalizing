namespace Pingring.AddressNormalization.Api.Models;


public sealed class VerifiedAddress
{
    public required string Address1 { get; init; }
    public required string Address2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string ZipCode { get; init; }
    public required string[] Flags { get; init; }
    public required double? Latitude { get; init; }
    public required double? Longitude { get; init; }
}

public sealed class UnverifiedAddress
{
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
}