namespace Pingring.AddressNormalization.Api.Services.Lob;

internal class LobVerifiedAddress
{
    public string Recipient { get; set; } = string.Empty;
    public string PrimaryLine { get; set; } = string.Empty;
    public string SecondaryLine { get; set; } = string.Empty;
    public bool ValidAddress { get; set; }
    public string Deliverability { get; set; } = string.Empty;
    public LobAddressComponents? Components { get; set; } = null;
    public LobDeliverabilityAnalysis? DeliverabilityAnalysis { get; set; } = null;
}