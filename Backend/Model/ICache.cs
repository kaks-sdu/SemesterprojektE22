using Microsoft.Extensions.Caching.Memory;

namespace Backend.Model;

public interface ICache<in TId, T>
    where TId : notnull
    where T : notnull
{
    /// <summary>
    /// Check if the cache contains an entry, and retrieve it, if it does
    /// </summary>
    /// <param name="key">The key of the entry</param>
    /// <param name="value">The returned entry if found, otherwise null</param>
    /// <returns>If the entry was in the cache or not</returns>
    public bool Contains(TId key, out T? value);

    /// <summary>
    /// Get a entry from the cache.
    /// If the entry is not already in the cache, load it and add it
    /// </summary>
    /// <param name="key">The key of the entry</param>
    /// <param name="supplier">A way to retrieve the entry, should it not already be in the cache</param>
    /// <param name="cacheTime">How long the entry should be cached for</param>
    /// <returns>The entry from the cache</returns>
    public Task<T> Get(TId key, Func<Task<T>> supplier, TimeSpan cacheTime);

    /// <summary>
    /// Remove a single entry from the cache
    /// </summary>
    /// <param name="key">The key of the element to remove</param>
    public void Remove(TId key);

    /// <summary>
    /// Destroy the whole cache
    /// </summary>
    public void Destroy();
}

internal class Cache<TId, T> : ICache<TId, T> where TId : notnull where T : notnull
{
    private readonly MemoryCache _cache;

    public Cache(MemoryCache cache)
    {
        _cache = cache;
    }

    public bool Contains(TId id, out T? value)
    {
        var found = _cache.TryGetValue(GetKey(id), out var result);
        if (found) value = (T)result!;
        else value = default;
        return found;
    }

    public async Task<T> Get(TId id, Func<Task<T>> supplier, TimeSpan cacheTime)
    {
        var key = GetKey(id);
        if (_cache.TryGetValue(key, out var result))
        {
            return (T)result!;
        }

        var response = await supplier.Invoke();

        _cache.Set(key, response, cacheTime);

        return response;
    }

    public void Remove(TId key)
    {
        _cache.Remove(GetKey(key));
    }

    public void Destroy()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Creates a unique key for the cache index.
    /// This ensures that different caches with same key does not collide
    /// </summary>
    /// <param name="key">The key for the cache object</param>
    /// <returns>The name of the key, formatted as ${name--keyType-valueType-key}</returns>
    private string GetKey(TId key)
    {
        var keyName = "";

        var keyType = typeof(TId).FullName;
        if (keyType is not null) keyName += $"{keyType}-";

        var valueType = typeof(T).FullName;
        if (valueType is not null) keyName += $"{valueType}-";

        return keyName + key;
    }
}