namespace Pingring.AddressNormalization.Api.Services.Lob;

internal class LobUnverifiedAddress
{
    public string Recipient { get; set; } = string.Empty;
    public string PrimaryLine { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}