using System.Text.Json.Serialization;

namespace Pingring.AddressNormalization.Api.Services.Lob;

internal class LobAddressComponents
{

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    
    [JsonPropertyName("zip_code")]
    public string ZipCode { get; set; } = string.Empty;

    [JsonPropertyName("zip_code_plus_4")]
    public string ZipCodePlus4 { get; set; } = string.Empty;

    [JsonPropertyName("zip_code_type")]
    public string ZipCodeType { get; set; } = string.Empty;

    [JsonPropertyName("address_type")]
    public string AddressType { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public float? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public float? Longitude { get; set; }
}

internal class LobDeliverabilityAnalysis
{
    [JsonPropertyName("dpv_footnotes")]
    public string[]? Footnotes { get; set; }

    [JsonPropertyName("dpv_vacant")]
    public string Vacant { get; set; } = string.Empty;
}