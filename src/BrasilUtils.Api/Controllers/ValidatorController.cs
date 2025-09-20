
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrasilUtils.Api.Controllers;

[ApiController]
[Route("api/validator")]
public class ValidatorController : ControllerBase
{
    private readonly CpfCnpjService _svc;
    public ValidatorController(CpfCnpjService svc) => _svc = svc;

    [HttpPost("cpf")]
    public ActionResult<ValidateResponse> ValidateCpf([FromBody] ValidateRequest req)
    {
        var (clean, ok) = _svc.ValidateCpf(req.Value);
        return Ok(new ValidateResponse(clean, ok));
    }

    [HttpPost("cnpj")]
    public ActionResult<ValidateResponse> ValidateCnpj([FromBody] ValidateRequest req)
    {
        var (clean, ok) = _svc.ValidateCnpj(req.Value);
        return Ok(new ValidateResponse(clean, ok));
    }
}
