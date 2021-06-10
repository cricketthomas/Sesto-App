using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace sesto.api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestRateLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }

        public int Seconds { get; set; }

        public static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            var clientAddress = context.HttpContext.Request.HttpContext.Connection.LocalIpAddress;

            var cacheKey = $"{Name}_{ipAddress}";

            if (!Cache.TryGetValue(cacheKey, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));
                Cache.Set(cacheKey, true, cacheEntryOptions);
            }
            else
            {
                context.Result = new ContentResult { Content = "429 too many requests.", StatusCode = (int)HttpStatusCode.TooManyRequests };
            }

        }

    }

}
