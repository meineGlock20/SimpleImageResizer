using System.Globalization;

namespace SimpleImageResizer.Core;

/// <summary>
/// Possible number of decimal places for a double.
/// </summary>
public enum RoundToDecimalPlaces
{
    Zero, One, Two, Three, Four, Five, Six, Seven, Eight,
    Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen
};


/// <summary>
/// Class that Calculates space.
/// </summary>
public static class Calculate
{
    // DF = double formatted using thousands seperator and on decimal place
    private const string DF = "#,#,0.00";

    /// <summary>
    /// Calculates Bytes to GigaBytes and returns a double to one decimal with unit of measure appended. IE: 9.1 GB.
    /// </summary>
    /// <param name="d">Bytes to be returned as GB.</param>
    /// <returns>Gets a string represeting GB.</returns>
    public static string BytesToGb(this double d)
    {
        return $"{(d / 2014 / 1024 / 1024).ToString(DF, CultureInfo.InvariantCulture)} GB";
    }

    /// <summary>
    /// Calculates Bytes to nearest unit of measure (KB, MB, GB, TB, PB) with
    /// thousands seperator formatting for culture and rounded as required as a human readable string.
    /// Default is one decimal place.
    /// </summary>
    /// <param name="bytes">Bytes to calculate.</param>
    /// <param name="culture">The culture for number formatting. Null for Invariant Culture. (Optional)</param>
    /// <param name="roundToDecimalPlaces">
    /// The number of decimal places to round the result. 0-15. Null for 1.
    /// Note that trailing zeros are removed. 1.00 KB will display as 1 KB. (Optional)
    /// </param>
    /// <returns>A string represting human readable size.</returns>
    public static string? ToDiskSpace(this long bytes, string? culture = null, RoundToDecimalPlaces? roundToDecimalPlaces = null)
    {
        const long kb = 1024;
        const long mb = 1048576;
        const long gb = 1073741824;
        const long tb = 1099511627776;
        const long pb = 1125899906842624;

        // Temporarily set negative numbers to positive. This is set back later.
        bool isNegative = false;
        if (bytes < 0)
        {
            isNegative = true;
            bytes = Math.Abs(bytes);
        }

        double d = bytes switch
        {
            < kb => bytes,
            < mb => (double)bytes / kb,
            < gb => (double)bytes / mb,
            < tb => (double)bytes / gb,
            < pb => (double)bytes / tb,
            _ => (double)bytes / pb,
        };

        // Round the result to required decimal places or default to 1 if not passed to a max of 15.
        int decimalPlaces = roundToDecimalPlaces is null ? 1 : (int)roundToDecimalPlaces;
        double result = Math.Round(d, (int)decimalPlaces);
        if (isNegative) { result = -result; }

        // If culture is not passed or the passed one does not exist, use Invariant.
        CultureInfo cultureInfo = culture is null || !cultureExists() ? CultureInfo.InvariantCulture : new(culture);
        bool cultureExists()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(x => x.Name.Equals(culture, StringComparison.OrdinalIgnoreCase));
        };

        // Format the string for the culture. Bytes will never have decimal places.
        // REM: "#,#,0" is used for bytes to force a 0 to display if the result == 0, otherwise it would be blank.
        // REM: The new string of # is for removing training zeros.
        return bytes switch
        {
            < kb => result == 1 ? "1 byte" : $"{result.ToString("#,#,0", cultureInfo)} bytes",
            < mb => $"{result.ToString($"#,#.{new string('#', decimalPlaces)}", cultureInfo)} KB",
            < gb => $"{result.ToString($"#,#.{new string('#', decimalPlaces)}", cultureInfo)} MB",
            < tb => $"{result.ToString($"#,#.{new string('#', decimalPlaces)}", cultureInfo)} GB",
            < pb => $"{result.ToString($"#,#.{new string('#', decimalPlaces)}", cultureInfo)} TB",
            _ => $"{result.ToString($"#,#.{new string('#', decimalPlaces)}", cultureInfo)} PB",
        };
    }
}