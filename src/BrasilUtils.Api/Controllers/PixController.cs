
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrasilUtils.Api.Controllers;

[ApiController]
[Route("api/pix")]
public class PixController : ControllerBase
{
    private readonly PixPayloadService _pix;
    public PixController(PixPayloadService pix) => _pix = pix;

    [HttpPost("static-payload")]
    public ActionResult<PixStaticPayloadResponse> StaticPayload([FromBody] PixStaticPayloadRequest req)
    {
        var res = _pix.BuildStaticPayload(req);
        return Ok(res);
    }
}
