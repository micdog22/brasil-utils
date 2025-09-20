
using System.Text.RegularExpressions;

namespace BrasilUtils.Api.Utils;

public static class Strings
{
    public static string OnlyDigits(string s) => Regex.Replace(s ?? string.Empty, "\\D", "");
    public static bool AllCharsSame(string s) => s.Length > 0 && s.All(c => c == s[0]);
}
