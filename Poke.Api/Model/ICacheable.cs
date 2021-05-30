using System;

namespace Poke.Api.Model
{
    public interface ICacheable
    {
        string CacheKey { get; }
        TimeSpan CacheDuration { get; }
    }
}