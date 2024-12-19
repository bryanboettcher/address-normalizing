using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pingring.AddressNormalization.Api.Models;
using Pingring.Common;

namespace Pingring.AddressNormalization.Api.Services.Lob;

[ExcludeFromAutoScan]
public sealed class LobAddressNormalizer : INormalizerService
{
    private static readonly Uri BulkVerifyUri = new("https://api.lob.com/v1/bulk/us_verifications");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        AllowTrailingCommas = true,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    public LobAddressNormalizer(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async IAsyncEnumerable<VerifiedAddress> Normalize(
        IEnumerable<UnverifiedAddress> input, 
        [EnumeratorCancellation] CancellationToken token)
    {
        var client = _httpClientFactory.CreateClient("LOB");
        var requestPayload = new LobBulkVerifyRequest
        {
            Addresses = input.Select(ConvertToLob).ToArray()
        };

        await using var buffer = new MemoryStream();
        await JsonSerializer.SerializeAsync(
            buffer,
            requestPayload,
            SerializerOptions,
            token
        );

        buffer.Position = 0;

        _logger.LogDebug("LOB payload is {size} bytes", buffer.Length);

        using var content = new StreamContent(buffer);
        var response = await client.PostAsync(
            BulkVerifyUri,
            content,
            token
        );

        var responsePayload = await ProcessResponseAsync<LobBulkVerifyResponse>(response, token);

        if (responsePayload.Addresses is null)
            throw new InvalidOperationException("LOB returned a null payload, but did not give an error");

        var output = responsePayload.Addresses.Select(ConvertFromLob).ToArray();

        for (var i = 0; i < input.Length; i++)
        {
            output[i].RecipientId = input[i].RecipientId;
            output[i].Tag = input[i].Tag;
        }
        return output;
    }

    private static async ValueTask<T> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken token)
    {
        var streamResult = await response.Content.ReadAsStreamAsync(token);
        if (!response.IsSuccessStatusCode)
        {
            var error = await JsonSerializer.DeserializeAsync<ErrorResponse>(
                streamResult,
                SerializerOptions,
                token
            );

            var lobError = error?.Error ?? new LobError { Message = "no error provided", StatusCode = -1 };

            throw (int)response.StatusCode switch
            {
                (int)System.Net.HttpStatusCode.Unauthorized => new UnauthorizedException(lobError.Message, lobError.StatusCode),
                (int)System.Net.HttpStatusCode.Forbidden => new ForbiddenException(lobError.Message, lobError.StatusCode),
                (int)System.Net.HttpStatusCode.NotFound => new NotFoundException(lobError.Message, lobError.StatusCode),
                (int)System.Net.HttpStatusCode.BadRequest => new BadRequestException(lobError.Message, lobError.StatusCode),
                422 => new BadRequestException(lobError.Message, lobError.StatusCode),
                429 => new TooManyRequestsException(lobError.Message, lobError.StatusCode),
                (int)System.Net.HttpStatusCode.InternalServerError => new ServerErrorException(lobError.Message, lobError.StatusCode),
                _ => new("An unexpected error occurred.")
            };
        }

        var output = await JsonSerializer.DeserializeAsync<T>(
            streamResult,
            SerializerOptions,
            token
        );

        return output ?? throw new InvalidOperationException("Deserializing the LOB response was completely null");
    }

    private static VerifiedAddress ConvertFromLob(LobVerifiedAddress arg)
    {
        if (arg.Components is null)
        {
            return new()
            {
                Address1 = string.Empty,
                Address2 = string.Empty,
                City = string.Empty,
                State = string.Empty,
                ZipCode = string.Empty,
                Flags = [],
                Latitude = null,
                Longitude = null,
            };
        }

        var addressFlags = BuildAddressFlags(arg);
        var zipCode = BuildZipCode(
            arg.Components.ZipCode, 
            arg.Components.ZipCodePlus4
        );

        return new()
        {
            Address1 = arg.PrimaryLine,
            Address2 = arg.SecondaryLine,
            City = arg.Components.City,
            State = arg.Components.State,
            ZipCode = zipCode,
            Flags = addressFlags,
            Latitude = arg.Components.Latitude,
            Longitude = arg.Components.Longitude,
        };
    }

    private static string BuildZipCode(string? zip, string? zipPlus4)
    {
        if (!int.TryParse(zip, out var parsedZip))
            return string.Empty;

        return int.TryParse(zipPlus4, out var parsedZipPlus4) 
            ? $"{parsedZip}-{parsedZipPlus4}" 
            : $"{parsedZip}";
    }

    private static string[] BuildAddressFlags(LobVerifiedAddress arg)
    {
        // hardcoded string comparison values are all from https://docs.lob.com/#tag/US-Verification-Types

        Debug.Assert(arg.Components is not null, "arg.Components is not null");

        string[] flags = [];

         arg.Components.ZipCodeType switch
        {
            "standard" => AddressFlags.ZipCodeStandard,
            "po_box" => AddressFlags.ZipCodePoBox,
            "unique" => AddressFlags.ZipCodeUnique,
            "military" => AddressFlags.ZipCodeMilitary,
            _ => AddressFlags.Unknown
        };

        flags |= arg.Components.AddressType switch
        {
            "residential" => AddressFlags.AddressResidential,
            "commercial" => AddressFlags.AddressCommercial,
            _ => AddressFlags.Unknown
        };

        flags |= arg.Deliverability switch
        {
            "deliverable" => AddressFlags.DeliverabilityIsDeliverable,
            "deliverable_unnecessary_unit" => AddressFlags.DeliverabilityRemoveSecondary,
            "deliverable_incorrect_unit" => AddressFlags.DeliverabilityIncorrectSecondary,
            "deliverable_missing_unit" => AddressFlags.DeliverabilityMissingSecondary,
            "undeliverable" => AddressFlags.DeliverabilityUspsUndeliverable,
            _ => AddressFlags.Unknown
        };

        flags |= arg.Deliverability switch
        {
            "deliverable" => AddressFlags.AddressDeliverable,
            "deliverable_unnecessary_unit" => AddressFlags.AddressDeliverable,
            _ => AddressFlags.Unknown
        };

        if (arg.ValidAddress)
            flags |= AddressFlags.AddressValid;

        if (arg.DeliverabilityAnalysis?.Vacant.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)
            flags |= AddressFlags.DeliverabilityIsVacant;

        if (arg.DeliverabilityAnalysis?.Footnotes?.Contains("IA", StringComparer.OrdinalIgnoreCase) ?? false)
            flags |= AddressFlags.DeliverabilityIsInformed;

        if (arg.DeliverabilityAnalysis?.Footnotes?.Contains("G1", StringComparer.OrdinalIgnoreCase) ?? false)
            flags |= AddressFlags.DeliverabilityIsGeneralDelivery;

        if (arg.DeliverabilityAnalysis?.Footnotes?.Contains("R7", StringComparer.OrdinalIgnoreCase) ?? false)
            flags |= AddressFlags.DeliverabilityIsPhantom;

        return flags;
    }

    private static LobUnverifiedAddress ConvertToLob(UnverifiedAddress input)
    {
        var primary = input.Address2 is not null
            ? $"{input.Address1} {input.Address2}"
            : input.Address1 ?? "";

        var stateShort = input.State?.Length == 2 
            ? input.State 
            : StateMap.FromState(input.State);

        return new()
        {
            Recipient = "",
            PrimaryLine = primary,
            City = input.City,
            State = stateShort,
            ZipCode = input.ZipCode
        };
    }
}

public static class AddressFlags
{
    public const string ZipCodeStandard = "ZIP_STANDARD";
    public const string ZipCodePoBox = "ZIP_POBOX";
    public const string ZipCodeUnique = "ZIP_UNIQUE";
}