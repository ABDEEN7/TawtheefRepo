namespace Tawtheef.Application.Extensions;

public static class CollectionExtensions
{
    public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        // Create a temporary list of items to remove to avoid modifying the collection during iteration
        var itemsToRemove = collection.Where(predicate).ToList();

        foreach (var item in itemsToRemove)
        {
            collection.Remove(item);
        }
    }
}
