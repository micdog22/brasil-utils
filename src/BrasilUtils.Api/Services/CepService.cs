
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Utils;

namespace BrasilUtils.Api.Services;

public class CepService
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _cfg;

    public CepService(IHttpClientFactory factory, IConfiguration cfg)
    {
        _factory = factory;
        _cfg = cfg;
    }

    public async Task<CepResponse?> GetAsync(string cep, CancellationToken ct = default)
    {
        var clean = Strings.OnlyDigits(cep);
        if (clean.Length != 8) throw new ArgumentException("CEP inválido");

        var baseUrl = _cfg["Viacep:BaseUrl"] ?? "https://viacep.com.br/ws/";
        var url = $"{baseUrl}{clean}/json/";
        var http = _factory.CreateClient();
        using var resp = await http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return null;
        var json = await resp.Content.ReadAsStringAsync(ct);
        var doc = System.Text.Json.JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("erro", out var erro) && erro.GetBoolean())
            return null;

        string get(string n) => doc.RootElement.TryGetProperty(n, out var v) ? v.GetString() ?? "" : "";
        return new CepResponse(
            get("cep"),
            get("logradouro"),
            get("complemento"),
            get("bairro"),
            get("localidade"),
            get("uf"),
            get("ibge"),
            get("gia"),
            get("ddd"),
            get("siafi")
        );
    }
}
