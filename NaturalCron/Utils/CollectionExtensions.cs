namespace NaturalCron.Utils;

internal static class CollectionExtensions
{
    internal static IList<T> AsList<T>(this T item) => new List<T>
    {
        item
    };
}   
