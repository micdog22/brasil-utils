
using BrasilUtils.Api.Utils;

namespace BrasilUtils.Api.Services;

public class CpfCnpjService
{
    public (string Clean, bool IsValid) ValidateCpf(string input)
    {
        var s = Strings.OnlyDigits(input);
        if (s.Length != 11 || Strings.AllCharsSame(s)) return (s, false);

        int[] w1 = {10,9,8,7,6,5,4,3,2};
        int[] w2 = {11,10,9,8,7,6,5,4,3,2};

        int sum1 = 0;
        for (int i=0;i<9;i++) sum1 += (s[i]-'0') * w1[i];
        int r1 = sum1 % 11;
        int d1 = r1 < 2 ? 0 : 11 - r1;

        int sum2 = 0;
        for (int i=0;i<10;i++) sum2 += (s[i]-'0') * w2[i];
        int r2 = sum2 % 11;
        int d2 = r2 < 2 ? 0 : 11 - r2;

        return (s, d1 == (s[9]-'0') && d2 == (s[10]-'0'));
    }

    public (string Clean, bool IsValid) ValidateCnpj(string input)
    {
        var s = Strings.OnlyDigits(input);
        if (s.Length != 14 || Strings.AllCharsSame(s)) return (s, false);

        int[] w1 = {5,4,3,2,9,8,7,6,5,4,3,2};
        int[] w2 = {6,5,4,3,2,9,8,7,6,5,4,3,2};

        int sum1 = 0;
        for (int i=0;i<12;i++) sum1 += (s[i]-'0') * w1[i];
        int r1 = sum1 % 11;
        int d1 = r1 < 2 ? 0 : 11 - r1;

        int sum2 = 0;
        for (int i=0;i<13;i++) sum2 += (s[i]-'0') * w2[i];
        int r2 = sum2 % 11;
        int d2 = r2 < 2 ? 0 : 11 - r2;

        return (s, d1 == (s[12]-'0') && d2 == (s[13]-'0'));
    }
}
