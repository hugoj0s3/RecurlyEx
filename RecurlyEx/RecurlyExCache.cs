using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RecurlyEx;

internal class RecurlyExCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> cache = new();
    private readonly TimeSpan expirationTimeSpan;
    private readonly int minSizeToCleanup;

    private readonly Timer cleanUpTimer;

    internal RecurlyExCache(TimeSpan expirationTimeSpan, int minSizeToCleanup)
    {
        this.expirationTimeSpan = expirationTimeSpan;
        this.minSizeToCleanup = minSizeToCleanup;
        var cleanupInterval = TimeSpan.FromTicks(expirationTimeSpan.Ticks / 3);
        cleanUpTimer = new Timer(CleanUp, null, cleanupInterval, cleanupInterval);
    }

    public void Set(string key, object value)
    {
        var entry = new CacheEntry
        {
            Key = key,
            Value = value,
            LastAccessed = DateTime.UtcNow
        };
        cache[key] = entry;
    }

    public object? Get(string key)
    {
        if (cache.TryGetValue(key, out var entry))
        {
            entry.LastAccessed = DateTime.UtcNow;
            return entry.Value;
        }
        
        return null;
    }

    private void CleanUp(object? state)
    {
        if (cache.Count < minSizeToCleanup)
        {
            return;
        }
        
        var now = DateTime.UtcNow;
        var keysToRemove = new List<string>();

        foreach (var kvp in cache)
        {
            if (now - kvp.Value.LastAccessed > expirationTimeSpan)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            cache.TryRemove(key, out _);
        }
    }
}

internal class CacheEntry
{
    public string Key { get; set; } = string.Empty;
    public object? Value { get; set; }
    public DateTime LastAccessed { get; set; }
}