namespace Pingring.AddressNormalization.Api.Services.Lob;

internal class ErrorResponse
{
    public LobError Error { get; set; } = new();
}