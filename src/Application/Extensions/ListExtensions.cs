namespace Tawtheef.Application.Extensions;

public static class ListExtensions
{
    extension<TSource>(List<TSource> source)
    {
        public List<TSource> WhereIf(bool condition,
            Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate).ToList() : source;
        }
    }
}
