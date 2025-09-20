
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrasilUtils.Api.Controllers;

[ApiController]
[Route("api/boleto")]
public class BoletoController : ControllerBase
{
    private readonly BoletoService _boleto;
    public BoletoController(BoletoService boleto) => _boleto = boleto;

    [HttpPost("barcode-dv")]
    public ActionResult<BoletoDvResponse> BarcodeDv([FromBody] BoletoDvRequest req)
    {
        var res = _boleto.ComputeBarcodeDv(req.Barcode43);
        return Ok(res);
    }
}
