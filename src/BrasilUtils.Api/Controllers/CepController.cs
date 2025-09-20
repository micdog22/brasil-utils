
using BrasilUtils.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrasilUtils.Api.Controllers;

[ApiController]
[Route("api/cep")]
public class CepController : ControllerBase
{
    private readonly CepService _cep;
    public CepController(CepService cep) => _cep = cep;

    [HttpGet("{cep}")]
    public async Task<IActionResult> Get(string cep, CancellationToken ct)
    {
        var res = await _cep.GetAsync(cep, ct);
        if (res is null) return NotFound(new { message = "CEP não encontrado ou inválido" });
        return Ok(res);
    }
}
