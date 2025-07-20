using System.Text.RegularExpressions;

namespace NaturalCron;

internal static class StringExtensions
{
    internal static string[] ToUpper(this string[] strings) =>
        strings.Select(x => x.ToUpper()).ToArray();
    
    internal static string[] ToLower(this string[] strings) =>
        strings.Select(x => x.ToLower()).ToArray();
    
    internal static bool IsWholeNumber(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return value.All(char.IsDigit);
    }

    public static bool ContainsWholeWord(this string input, string word)
    {
        return Regex.IsMatch(input, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase);
    }
    public static string NormalizeExpression(this string expression)
    {
        return Regex.Replace(expression, @"\s*([+-])\s*", "$1").Trim();
    }
}