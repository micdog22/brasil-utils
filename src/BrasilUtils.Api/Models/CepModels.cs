
namespace BrasilUtils.Api.Models;

public record CepResponse(
    string cep,
    string logradouro,
    string complemento,
    string bairro,
    string localidade,
    string uf,
    string ibge,
    string gia,
    string ddd,
    string siafi
);
