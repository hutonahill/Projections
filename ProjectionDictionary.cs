namespace Projections;

using System.Diagnostics.CodeAnalysis;
using System.Collections;

/// <summary>
/// Displays a view of a dictionary.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the accessible value.</typeparam>
/// <typeparam name="TSource">The source of the inaccessible value.</typeparam>
public sealed class ProjectionDictionary<TKey, TValue, TSource> : IReadOnlyDictionary<TKey, TValue> {
    private readonly IDictionary<TKey, TSource> _source;
    private readonly Func<TSource, TValue> _projector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionDictionary{TKey, TValue, TSource}"/> class.
    /// </summary>
    /// <param name="source">The source dictionary.</param>
    /// <param name="convert">Function to convert the internal source type to the external type.</param>
    public ProjectionDictionary(IDictionary<TKey, TSource> source, Func<TSource, TValue> convert) {
        _source = source;
        _projector = convert;
    }

    /// <inheritdoc/>
    public TValue this[TKey key] => _projector(_source[key]);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        foreach (KeyValuePair<TKey, TSource> kvp in _source) {
            yield return new KeyValuePair<TKey, TValue>(kvp.Key, _projector(kvp.Value));
        }
    }

    /*// Example use case
     private static Dictionary<int, int> testSource = new Dictionary<int, int>();

    private static ProjectionDictionary<int, string, int> test = new (testSource,
        i => i.ToString());*/

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public int Count => _source.Count;

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) {
        return _source.ContainsKey(key);
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        if (_source.TryGetValue(key, out TSource? temp)) {
            value = _projector(temp);
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys => _source.Keys;

    /// <inheritdoc/>
    public IEnumerable<TValue> Values {
        get {
            foreach (TSource sourceValue in _source.Values) {
                yield return _projector(sourceValue);
            }
        }
    }
}