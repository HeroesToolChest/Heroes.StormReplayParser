namespace Heroes.StormReplayParser;

/// <summary>
/// A custom <see cref="List{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public class StormDataStructure<T> : List<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StormDataStructure{T}"/> class.
    /// </summary>
    public StormDataStructure()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormDataStructure{T}"/> class.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public StormDataStructure(IEnumerable<T> collection)
        : base(collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StormDataStructure{T}"/> class.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>.</exception>
    public StormDataStructure(int capacity)
        : base(capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);
    }

    /// <summary>
    /// Gets or sets the element at the specified index. The item is added if the <paramref name="index"/>
    /// is currently uninitialized.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    public new T this[int index]
    {
        get
        {
            return base[index];
        }
        set
        {
            if (index >= Count)
                Add(value);
            else
                base[index] = value;
        }
    }
}
