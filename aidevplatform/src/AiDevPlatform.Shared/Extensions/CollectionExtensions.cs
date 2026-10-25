namespace AiDevPlatform.Shared.Extensions;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> collection) where T : class
    {
        return collection.Where(item => item != null)!;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> collection) where T : struct
    {
        return collection.Where(item => item.HasValue).Select(item => item!.Value);
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
    {
        return collection.GroupBy(keySelector).Select(group => group.First());
    }

    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> collection, string propertyName, bool descending = false)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");

        return descending
            ? collection.OrderByDescending(item => property.GetValue(item))
            : collection.OrderBy(item => property.GetValue(item));
    }

    public static IEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> collection, string propertyName, bool descending = false)
    {
        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");

        return descending
            ? collection.ThenByDescending(item => property.GetValue(item))
            : collection.ThenBy(item => property.GetValue(item));
    }

    public static IEnumerable<T> TakePage<T>(this IEnumerable<T> collection, int pageNumber, int pageSize)
    {
        return collection.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var random = new Random();
        return collection.OrderBy(_ => random.Next());
    }

    public static IEnumerable<T> Random<T>(this IEnumerable<T> collection, int count)
    {
        return collection.Shuffle().Take(count);
    }

    public static T? RandomOrDefault<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        if (list.Count == 0)
            return default;

        var random = new Random();
        return list[random.Next(list.Count)];
    }

    public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> collection, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be greater than 0", nameof(chunkSize));

        var chunk = new List<T>(chunkSize);
        foreach (var item in collection)
        {
            chunk.Add(item);
            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }

        if (chunk.Count > 0)
            yield return chunk;
    }

    public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(
        this IEnumerable<T> collection,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        return collection.ToDictionary(keySelector, valueSelector, comparer);
    }

    public static ILookup<TKey, TValue> ToLookup<T, TKey, TValue>(
        this IEnumerable<T> collection,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        return collection.ToLookup(keySelector, valueSelector, comparer);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> collection)
    {
        return collection.SelectMany(item => item);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>?> childrenSelector)
    {
        foreach (var item in collection)
        {
            yield return item;
            var children = childrenSelector(item);
            if (children != null)
            {
                foreach (var child in children.Flatten(childrenSelector))
                {
                    yield return child;
                }
            }
        }
    }

    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> collection, Func<T, IEnumerable<T>?> childrenSelector)
    {
        return collection.Flatten(childrenSelector);
    }

    public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, IEnumerable<T> other, Func<T, T, bool> comparer)
    {
        return collection.Where(item => !other.Any(otherItem => comparer(item, otherItem)));
    }

    public static IEnumerable<T> Intersect<T>(this IEnumerable<T> collection, IEnumerable<T> other, Func<T, T, bool> comparer)
    {
        return collection.Where(item => other.Any(otherItem => comparer(item, otherItem)));
    }

    public static IEnumerable<T> Union<T>(this IEnumerable<T> collection, IEnumerable<T> other, Func<T, T, bool> comparer)
    {
        var result = new List<T>();
        foreach (var item in collection)
        {
            if (!result.Any(resultItem => comparer(item, resultItem)))
                result.Add(item);
        }

        foreach (var item in other)
        {
            if (!result.Any(resultItem => comparer(item, resultItem)))
                result.Add(item);
        }

        return result;
    }

    public static bool SequenceEqual<T>(this IEnumerable<T> collection, IEnumerable<T> other, Func<T, T, bool> comparer)
    {
        var collectionList = collection.ToList();
        var otherList = other.ToList();

        if (collectionList.Count != otherList.Count)
            return false;

        for (int i = 0; i < collectionList.Count; i++)
        {
            if (!comparer(collectionList[i], otherList[i]))
                return false;
        }

        return true;
    }

    public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> collection, Func<T, int, bool> predicate)
    {
        var index = 0;
        foreach (var item in collection)
        {
            if (!predicate(item, index))
                yield return item;
            index++;
        }
    }

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> collection, Func<T, int, bool> predicate)
    {
        var index = 0;
        foreach (var item in collection)
        {
            if (predicate(item, index))
                yield return item;
            else
                break;
            index++;
        }
    }

    public static IEnumerable<T> Where<T>(this IEnumerable<T> collection, Func<T, int, bool> predicate)
    {
        var index = 0;
        foreach (var item in collection)
        {
            if (predicate(item, index))
                yield return item;
            index++;
        }
    }

    public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> collection, Func<T, int, TResult> selector)
    {
        var index = 0;
        foreach (var item in collection)
        {
            yield return selector(item, index);
            index++;
        }
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in collection)
        {
            action(item, index);
            index++;
        }
    }

    public static T? FirstOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> predicate, T? defaultValue = default)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
                return item;
        }
        return defaultValue;
    }

    public static T? LastOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> predicate, T? defaultValue = default)
    {
        var last = default(T);
        var found = false;

        foreach (var item in collection)
        {
            if (predicate(item))
            {
                last = item;
                found = true;
            }
        }

        return found ? last : defaultValue;
    }

    public static T? SingleOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> predicate, T? defaultValue = default)
    {
        var result = default(T);
        var found = false;

        foreach (var item in collection)
        {
            if (predicate(item))
            {
                if (found)
                    throw new InvalidOperationException("Sequence contains more than one matching element");
                result = item;
                found = true;
            }
        }

        return found ? result : defaultValue;
    }

    public static bool Any<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
                return true;
        }
        return false;
    }

    public static bool All<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (!predicate(item))
                return false;
        }
        return true;
    }

    public static int Count<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        var count = 0;
        foreach (var item in collection)
        {
            if (predicate(item))
                count++;
        }
        return count;
    }

    public static long LongCount<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
    {
        long count = 0;
        foreach (var item in collection)
        {
            if (predicate(item))
                count++;
        }
        return count;
    }

    public static T? Min<T>(this IEnumerable<T> collection, Func<T, IComparable> selector)
    {
        var min = default(T);
        var minValue = default(IComparable);
        var first = true;

        foreach (var item in collection)
        {
            var value = selector(item);
            if (first || value.CompareTo(minValue) < 0)
            {
                min = item;
                minValue = value;
                first = false;
            }
        }

        return min;
    }

    public static T? Max<T>(this IEnumerable<T> collection, Func<T, IComparable> selector)
    {
        var max = default(T);
        var maxValue = default(IComparable);
        var first = true;

        foreach (var item in collection)
        {
            var value = selector(item);
            if (first || value.CompareTo(maxValue) > 0)
            {
                max = item;
                maxValue = value;
                first = false;
            }
        }

        return max;
    }

    public static decimal Average<T>(this IEnumerable<T> collection, Func<T, decimal> selector)
    {
        var sum = 0m;
        var count = 0;

        foreach (var item in collection)
        {
            sum += selector(item);
            count++;
        }

        return count > 0 ? sum / count : 0;
    }

    public static decimal Sum<T>(this IEnumerable<T> collection, Func<T, decimal> selector)
    {
        var sum = 0m;
        foreach (var item in collection)
        {
            sum += selector(item);
        }
        return sum;
    }
}
