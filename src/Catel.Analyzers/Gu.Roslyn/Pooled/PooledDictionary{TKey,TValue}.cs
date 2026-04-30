#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// A <see cref="Dictionary{TKey,TValue}"/> for re-use.
/// </summary>
/// <typeparam name="TKey">The type of keys.</typeparam>
/// <typeparam name="TValue">The type of values.</typeparam>
public sealed class PooledDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
    where TKey : notnull
{
    private static readonly ConcurrentQueue<PooledDictionary<TKey, TValue>> Cache = new();

    private readonly Dictionary<TKey, TValue> inner = new(GetComparer());

    private PooledDictionary()
    {
    }

    /// <inheritdoc />
    public int Count => inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => ((IDictionary<TKey, TValue>)inner).IsReadOnly;

    /// <inheritdoc />
    public ICollection<TKey> Keys => inner.Keys;

    /// <inheritdoc />
    public ICollection<TValue> Values => inner.Values;

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get => inner[key];
        set => inner[key] = value;
    }

    /// <summary>
    /// Borrow a dictionary, dispose returns it.
    /// </summary>
    /// <returns>A <see cref="PooledDictionary{TKey,TValue}"/>.</returns>
    public static PooledDictionary<TKey, TValue> Borrow()
    {
        if (Cache.TryDequeue(out var dictionary))
        {
            return dictionary;
        }

        return new PooledDictionary<TKey, TValue>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        inner.Clear();
        Cache.Enqueue(this);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => inner.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)inner).Add(item);

    /// <inheritdoc />
    public void Clear() => inner.Clear();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) => inner.Contains(item);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)inner).CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)inner).Remove(item);

    /// <inheritdoc />
    public void Add(TKey key, TValue value) => inner.Add(key, value);

    /// <inheritdoc />
    public bool ContainsKey(TKey key) => inner.ContainsKey(key);

    /// <inheritdoc />
    public bool Remove(TKey key) => inner.Remove(key);

    /// <inheritdoc />
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => inner.TryGetValue(key, out value);

    private static IEqualityComparer<TKey> GetComparer()
    {
        return PooledSet.SymbolComparers.OfType<IEqualityComparer<TKey>>().FirstOrDefault() ??
               EqualityComparer<TKey>.Default;
    }
}
