#pragma warning disable CA1000 // Do not declare static members on generic types
namespace Gu.Roslyn.AnalyzerExtensions;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// A <see cref="List{T}"/> for re-use.
/// </summary>
/// <typeparam name="T">The type of items.</typeparam>
public sealed class PooledList<T> : IList<T>, IDisposable
{
    private static readonly ConcurrentQueue<PooledList<T>> Cache = new();

    private readonly List<T> inner = new();

    private PooledList()
    {
    }

    /// <inheritdoc />
    public int Count => inner.Count;

    /// <inheritdoc />
    public bool IsReadOnly => ((IList<T>)inner).IsReadOnly;

    /// <inheritdoc />
    public T this[int index]
    {
        get => inner[index];
        set => inner[index] = value;
    }

    /// <summary>
    /// Borrow a dictionary, dispose returns it.
    /// </summary>
    /// <returns>A <see cref="PooledList{T}"/>.</returns>
    public static PooledList<T> Borrow()
    {
        if (Cache.TryDequeue(out var dictionary))
        {
            return dictionary;
        }

        return new PooledList<T>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        inner.Clear();
        Cache.Enqueue(this);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(T item) => inner.Add(item);

    /// <inheritdoc />
    public void Clear() => inner.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => inner.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => inner.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => inner.Remove(item);

    /// <inheritdoc />
    public int IndexOf(T item) => inner.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, T item) => inner.Insert(index, item);

    /// <inheritdoc />
    public void RemoveAt(int index) => inner.RemoveAt(index);
}
