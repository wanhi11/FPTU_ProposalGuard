using System.Globalization;
using System.Text.RegularExpressions;

namespace FPTU_ProposalGuard.Application.Utils;

public static class StringUtils
{
    private static readonly Random _rnd = new Random();

    // Generate unique code with specific length
    public static string GenerateUniqueCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_rnd.Next(s.Length)])
            .ToArray());
    }
    
    // Generate random code digits
    public static int GenerateRandomCodeDigits(int length)
    {
        return int.Parse(GenerateRandomDigitsWithTimeStamp(length));
    }
    
    // Generate random digits with specific time stamp
    private static string GenerateRandomDigitsWithTimeStamp(int length)
    {
        var rnd = new Random();

        // Get a timestamp (ticks)
        long timestamp = DateTime.Now.Ticks;

        // Use the last part of the timestamp to ensure limited size 
        string timestampPart = timestamp.ToString().Substring(timestamp.ToString().Length - Math.Min(8, length));

        // Generate the random digits portion
        string digits = string.Empty;
        for (int i = 0; i < length - timestampPart.Length; ++i)
        {
            digits += rnd.Next(0, 10); 
        }

        // Combine random digits with timestamp part
        return digits + timestampPart;
    }
    
    // Formats a string by replacing placeholders like <0>, <1>, etc., with the provided arguments.
    public static string Format(string input, params string[]? args)
    {
        if (string.IsNullOrEmpty(input))
            return null!;

        if (args == null || args.Length == 0)
            return input; // Return original string if no args provided.

        for (int i = 0; i < args.Length; i++)
        {
            input = input.Replace($"<{i}>", args[i]);
        }

        return input;
    }

    // Add white space to string
    public static string AddWhitespaceToString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Use a regex to identify boundaries between lowercase and uppercase letters
        string result = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");

        return result;
    }

    // Remove word and add white space to string
    public static string RemoveWordAndAddWhitespace(string input, string wordToRemove)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(wordToRemove))
            return input;

        // Remove the specified word
        string withoutWord = Regex.Replace(input, wordToRemove, "", RegexOptions.IgnoreCase);

        // Add whitespace to the remaining string
        return AddWhitespaceToString(withoutWord);
    }

    // Convert string to CamelCase
    public static string ToCamelCase(string s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
        {
            return s;
        }

        var chars = s.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = (i + 1 < chars.Length);
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
        }

        return new string(chars);
    }

    // Validate numeric & datetime
    public static bool IsDecimal(string text) => decimal.TryParse(text, out _);
    public static bool IsNumeric(string text) => int.TryParse(text, out _);

    public static bool IsDateTime(string text)
    {
        string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };
        return DateTime.TryParseExact(text, formats, null, DateTimeStyles.None, out _);
    }

    // Validate Http/Https Url
    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    // Validate prefix code 
    public static bool IsValidBarcodeWithPrefix(string barcode, string prefix)
    {
        return Regex.IsMatch(barcode, $@"^{prefix}\d+$");
    }

    // Get Public Id from Url 
    public static string? GetPublicIdFromUrl(string url)
    {
        // Check whether URL is valid
        if (!IsValidUrl(url)) return null;

        var lastSlashIndex = url.LastIndexOf('/');
        var lastDotIndex = url.LastIndexOf('.');

        // Check if both slash and dot exist in the URL
        if (lastSlashIndex == -1 || lastDotIndex == -1 || lastDotIndex < lastSlashIndex) return null;

        // Extract the public ID
        return url.Substring(lastSlashIndex + 1, lastDotIndex - lastSlashIndex - 1);
    }

    /// <summary>
    /// Normalize input text (lowercase and split into words)
    /// </summary>
    private static List<string> NormalizeText(string text)
    {
        return text.ToLower()
            .Split(new[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}