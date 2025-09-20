
# Brasil Utils API (C# • .NET 8)

Microserviço utilitário para o ecossistema brasileiro, com endpoints práticos e prontos para produção leve:
- **Validação** e normalização de **CPF** e **CNPJ**.
- **Consulta de CEP** via ViaCEP (proxy com normalização de resposta).
- **Geração de payload PIX estático** conforme BR Code, incluindo **CRC16** e **QR Code** em PNG (Base64).
- **Cálculo do Dígito Verificador** do **código de barras de boletos** (43 → 44, módulo 11 com pesos 2..9).

Inclui **Swagger/OpenAPI**, **testes xUnit**, **Dockerfile**, **GitHub Actions** e **licença MIT**.

---

## Sumário
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Como executar localmente](#como-executar-localmente)
- [Como executar via Docker](#como-executar-via-docker)
- [Configuração](#configuração)
- [Endpoints](#endpoints)
  - [Validação de CPF/CNPJ](#validação-de-cpfcnpj)
  - [Consulta de CEP](#consulta-de-cep)
  - [PIX: payload estático + QR](#pix-payload-estático--qr)
  - [Boleto: DV do código de barras](#boleto-dv-do-código-de-barras)
- [Exemplos com curl](#exemplos-com-curl)
- [Testes](#testes)
- [Boas práticas e segurança](#boas-práticas-e-segurança)
- [Roadmap](#roadmap)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

---

## Arquitetura

```
brasil-utils-api/
├─ src/BrasilUtils.Api/
│  ├─ Program.cs
│  ├─ Controllers/
│  │  ├─ ValidatorController.cs
│  │  ├─ CepController.cs
│  │  ├─ PixController.cs
│  │  └─ BoletoController.cs
│  ├─ Services/
│  │  ├─ CpfCnpjService.cs
│  │  ├─ CepService.cs
│  │  ├─ PixPayloadService.cs
│  │  └─ BoletoService.cs
│  ├─ Models/ (DTOs)
│  ├─ Utils/  (Crc16, Strings)
│  └─ appsettings.json
├─ tests/BrasilUtils.Api.Tests/
│  └─ BasicTests.cs
├─ Dockerfile
├─ .github/workflows/dotnet.yml
├─ BrasilUtils.sln
└─ README.md
```

Principais bibliotecas:
- ASP.NET Core 8 (Minimal Hosting, Controllers)
- Swashbuckle.AspNetCore (Swagger/OpenAPI)
- QRCoder (geração de QR em PNG)

---

## Pré-requisitos

- **.NET 8 SDK** para desenvolvimento local.
- Opcional: **Docker** para containerizar.
- Opcional: **Git** e **Visual Studio/VS Code**.

---

## Como executar localmente

Na raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet test

dotnet run --project src/BrasilUtils.Api
```

Aplicação: `http://localhost:5189`  
Swagger UI: `http://localhost:5189/swagger`

---

## Como executar via Docker

```bash
docker build -t brasil-utils-api:latest .
docker run --rm -p 5189:8080 brasil-utils-api:latest
# Swagger: http://localhost:5189/swagger
```

---

## Configuração

Arquivo: `src/BrasilUtils.Api/appsettings.json`

```json
{
  "Viacep": {
    "BaseUrl": "https://viacep.com.br/ws/"
  },
  "Pix": {
    "PointOfInitiationMethod": "11"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Você pode sobrescrever por **variáveis de ambiente**, por exemplo:
- `Viacep__BaseUrl`
- `Pix__PointOfInitiationMethod`

Em produção, recomenda-se adicionar **rate limiting**, **cache** (ex.: Redis para CEP) e **observabilidade** (Health Checks, logs estruturados).

---

## Endpoints

### Validação de CPF/CNPJ

- `POST /api/validator/cpf`  
  **Body**: `{ "value": "390.533.447-05" }`  
  **Resposta**: `{ "clean": "39053344705", "isValid": true }`

- `POST /api/validator/cnpj`  
  **Body**: `{ "value": "45.723.174/0001-10" }`  
  **Resposta**: `{ "clean": "45723174000110", "isValid": true }`

### Consulta de CEP

- `GET /api/cep/{cep}`  
  Exemplo: `/api/cep/01001000`  
  **Resposta** (resumo): `cep`, `logradouro`, `bairro`, `localidade`, `uf`, `ddd`, etc.  
  Observação: o serviço consome o **ViaCEP** e normaliza erros (404 quando não encontrado).

### PIX: payload estático + QR

- `POST /api/pix/static-payload`  
  **Body**:
  ```json
  {
    "key": "email@exemplo.com",
    "merchantName": "Loja Exemplo",
    "merchantCity": "SAO PAULO",
    "amount": 123.45,
    "description": "Pedido 123",
    "referenceLabel": "PEDIDO123"
  }
  ```
  **Resposta**:
  ```json
  {
    "payload": "00020101021126...6304ABCD",
    "qrPngBase64": "iVBORw0KGgoAAA..."
  }
  ```
  Observações:
  - Gera o payload EMV conforme BR Code com `CRC16`.
  - O QR é retornado em PNG, codificado em Base64.

### Boleto: DV do código de barras

- `POST /api/boleto/barcode-dv`  
  **Body**: `{ "barcode43": "<43_digitos_sem_dv>" }`  
  **Resposta**: `{ "dv": 7, "barcode44": "<43_digitos>7" }`  
  Observação: regra de **módulo 11** com pesos 2..9 da direita para a esquerda; resultados 0, 1 ou 10 mapeados para DV=1. Consulte o banco/convênio para variações.

---

## Exemplos com curl

```bash
# CPF
curl -s http://localhost:5189/api/validator/cpf -H "Content-Type: application/json"  -d '{"value":"390.533.447-05"}' | jq

# CNPJ
curl -s http://localhost:5189/api/validator/cnpj -H "Content-Type: application/json"  -d '{"value":"45.723.174/0001-10"}' | jq

# CEP
curl -s http://localhost:5189/api/cep/01001000 | jq

# PIX
curl -s http://localhost:5189/api/pix/static-payload -H "Content-Type: application/json"  -d '{"key":"email@exemplo.com","merchantName":"Loja Exemplo","merchantCity":"SAO PAULO","amount":123.45,"description":"Pedido 123","referenceLabel":"PEDIDO123"}' | jq

# BOLETO
curl -s http://localhost:5189/api/boleto/barcode-dv -H "Content-Type: application/json"  -d '{ "barcode43": "0019050095401448160690680935031433737003" }' | jq
```

---

## Testes

```bash
dotnet test
```
Os testes incluem integração com `WebApplicationFactory`, cobrindo validação de CPF e geração de payload PIX básico.

---

## Boas práticas e segurança

- Adicione **rate limiting** para proteger de abuso.
- Considere **cache** para a consulta de CEP.
- Revise as **dependências** (ex.: QRCoder) antes de produção.
- Configure **observabilidade**: logs estruturados, métricas, health checks.
- Quando expor publicamente, implemente autenticação/autorizações conforme seu cenário.

---

## Roadmap

- Cache em memória/Redis para CEP com expiração.
- Rate limiting nativo do ASP.NET Core.
- Payload PIX dinâmico.
- Validadores adicionais: telefone BR, RENAVAM, PIS/PASEP.
- Docker Compose com Redis.
- Exemplos de clientes em JavaScript e C#.

---

## Contribuindo

Contribuições são bem-vindas. Abra uma Issue ou envie um Pull Request com descrição clara do problema/solução, testes e documentação atualizada.

---

## Licença

Distribuído sob a **MIT License**. Consulte o arquivo `LICENSE`.
