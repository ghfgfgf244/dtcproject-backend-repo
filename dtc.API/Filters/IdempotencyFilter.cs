using dtc.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace dtc.API.Filters
{
    /// <summary>
    /// Idempotency Filter - prevents duplicate request processing.
    /// Add [ServiceFilter(typeof(IdempotencyFilter))] to any controller action you want to protect.
    /// The client must include an "Idempotency-Key" header (a unique UUID per request).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IdempotencyFilter : Attribute, IAsyncActionFilter
    {
        private const string HeaderKey = "Idempotency-Key";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderKey, out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
            {
                context.Result = new BadRequestObjectResult(new { message = $"Header '{HeaderKey}' is required for this operation." });
                return;
            }

            var cacheKey = $"idempotency:{idempotencyKey}";

            // Cache HIT: Already processed this request
            var cached = await cacheService.GetAsync<object>(cacheKey);
            if (cached != null)
            {
                context.Result = new ObjectResult(cached) { StatusCode = (int)HttpStatusCode.OK };
                return;
            }

            // Execute the action
            var executedContext = await next();

            // Cache the result if action was successful (2xx status)
            if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode is >= 200 and < 300)
            {
                await cacheService.SetAsync(cacheKey, objectResult.Value, CacheExpiry);
            }
        }
    }
}
