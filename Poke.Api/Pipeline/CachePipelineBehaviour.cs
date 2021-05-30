using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Poke.Api.Model;

namespace Poke.Api.Pipeline
{
    public class CachePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMemoryCache _cache;

        public CachePipelineBehaviour(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            //If you inherited your data class from ICachable, it will check cache from that code before execute data code and query
            //Currently it just uses app server memory cache, however you can combine it with distributed cache system like Redis or Memcached
            if (!(request is ICacheable cacheable))
            {
                return await next();
            }

            var isExist = _cache.TryGetValue(cacheable.CacheKey, out TResponse response);
            if (isExist)
            {
                return response;
            }
            
            response = await next();
            _cache.Set(cacheable.CacheKey, response, cacheable.CacheDuration);
            return response;
        }
    }
}