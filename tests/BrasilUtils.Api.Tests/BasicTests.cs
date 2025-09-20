
using System.Net.Http.Json;
using BrasilUtils.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace BrasilUtils.Api.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public BasicTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Cpf_Should_Validate_Known_Sample()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/validator/cpf", new ValidateRequest("390.533.447-05"));
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<ValidateResponse>();
        body!.IsValid.Should().BeTrue();
        body.Clean.Should().Be("39053344705");
    }

    [Fact]
    public async Task Pix_Should_Build_Payload()
    {
        var client = _factory.CreateClient();
        var req = new PixStaticPayloadRequest("email@exemplo.com","Loja Exemplo","SAO PAULO", 10.50m, "Pedido 1", "PED1");
        var res = await client.PostAsJsonAsync("/api/pix/static-payload", req);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<PixStaticPayloadResponse>();
        body!.Payload.Should().Contain("br.gov.bcb.pix");
        body.QrPngBase64.Should().NotBeNullOrEmpty();
    }
}
