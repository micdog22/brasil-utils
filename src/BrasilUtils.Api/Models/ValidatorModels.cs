
namespace BrasilUtils.Api.Models;

public record ValidateRequest(string Value);
public record ValidateResponse(string Clean, bool IsValid);
