
using BrasilUtils.Api.Models;
using BrasilUtils.Api.Utils;

namespace BrasilUtils.Api.Services;

public class BoletoService
{
    public BoletoDvResponse ComputeBarcodeDv(string barcode43)
    {
        var s = Strings.OnlyDigits(barcode43);
        if (s.Length != 43) throw new ArgumentException("barcode43 deve ter 43 dígitos");

        int weight = 2;
        int sum = 0;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            int n = s[i] - '0';
            sum += n * weight;
            weight++;
            if (weight > 9) weight = 2;
        }
        int mod = sum % 11;
        int dv = 11 - mod;
        if (dv == 0 || dv == 1 || dv == 10) dv = 1;

        return new BoletoDvResponse(dv, s.Insert(4, dv.ToString()));
    }
}
