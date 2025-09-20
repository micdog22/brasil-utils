
namespace BrasilUtils.Api.Models;

public record PixStaticPayloadRequest(
    string Key,
    string MerchantName,
    string MerchantCity,
    decimal? Amount,
    string? Description,
    string? ReferenceLabel
);

public record PixStaticPayloadResponse(string Payload, string QrPngBase64);
