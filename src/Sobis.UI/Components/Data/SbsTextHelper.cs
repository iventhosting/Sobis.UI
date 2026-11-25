using System.Text;

namespace Sobis.UI;

/// <summary>
/// Internal Turkish-aware text normalization and comparison helpers so Sobis.UI has zero external dependencies.
/// </summary>
internal static class SbsTextHelper
{
    /// <summary>
    /// Normalizes Turkish diacritics and casing to ASCII lowercase ("SİBEL" -> "sibel", "ŞAHİN" -> "sahin").
    /// </summary>
    public static string NormalizeTr(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            sb.Append(c switch
            {
                'İ' or 'I' or 'ı' or 'i' or 'Î' or 'î' => 'i',
                'Ş' or 'ş' => 's',
                'Ç' or 'ç' => 'c',
                'Ğ' or 'ğ' => 'g',
                'Ü' or 'ü' or 'Û' or 'û' => 'u',
                'Ö' or 'ö' or 'Ô' or 'ô' => 'o',
                'Â' or 'â' => 'a',
                _ => char.ToLowerInvariant(c)
            });
        }

        return sb.ToString();
    }

    public static bool ContainsTr(string? haystack, string? needle)
    {
        if (string.IsNullOrEmpty(haystack) || string.IsNullOrEmpty(needle))
            return false;

        return NormalizeTr(haystack).Contains(NormalizeTr(needle), StringComparison.Ordinal);
    }

    public static bool EqualsTr(string? a, string? b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;

        return string.Equals(NormalizeTr(a), NormalizeTr(b), StringComparison.Ordinal);
    }
}
