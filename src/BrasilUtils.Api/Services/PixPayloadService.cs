
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Utils;
using QRCoder;

namespace BrasilUtils.Api.Services;

public class PixPayloadService
{
    private readonly IConfiguration _cfg;
    public PixPayloadService(IConfiguration cfg) { _cfg = cfg; }

    public PixStaticPayloadResponse BuildStaticPayload(PixStaticPayloadRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Key)) throw new ArgumentException("Pix key é obrigatória");
        if (string.IsNullOrWhiteSpace(req.MerchantName)) throw new ArgumentException("merchantName é obrigatório");
        if (string.IsNullOrWhiteSpace(req.MerchantCity)) throw new ArgumentException("merchantCity é obrigatório");

        string pimm = _cfg["Pix:PointOfInitiationMethod"] ?? "11";

        static string TLV(string id, string value)
        {
            var len = value.Length.ToString("D2");
            return $"{id}{len}{value}";
        }

        string gui = TLV("00", "br.gov.bcb.pix");
        string key = TLV("01", req.Key.Trim());
        string desc = string.IsNullOrWhiteSpace(req.Description) ? "" : TLV("02", req.Description!.Trim());
        string mai = TLV("26", gui + key + desc);

        string mcc = TLV("52", "0000");
        string cur = TLV("53", "986");
        string amt = req.Amount.HasValue ? TLV("54", req.Amount.Value.ToString("0.00").Replace(',', '.')) : "";
        string cty = TLV("58", "BR");
        string name = TLV("59", req.MerchantName.Length > 25 ? req.MerchantName[..25] : req.MerchantName);
        string city = TLV("60", req.MerchantCity.Length > 15 ? req.MerchantCity[..15] : req.MerchantCity);

        string addData = "";
        if (!string.IsNullOrWhiteSpace(req.ReferenceLabel))
        {
            string refTLV = TLV("05", req.ReferenceLabel);
            addData = TLV("62", refTLV);
        }

        string payloadNoCrc = TLV("00", "01") + TLV("01", pimm) + mai + mcc + cur + amt + cty + name + city + addData + "6304";
        string crc = Crc16.ComputeHex(payloadNoCrc);
        string full = payloadNoCrc + crc;

        using var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode(full, QRCodeGenerator.ECCLevel.M);
        using var qr = new PngByteQRCode(data);
        var png = qr.GetGraphic(10);
        string b64 = Convert.ToBase64String(png);

        return new PixStaticPayloadResponse(full, b64);
    }
}
