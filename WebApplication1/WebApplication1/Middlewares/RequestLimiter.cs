namespace WebApplication1.Middlewares
{
    public static class RequestLimiter
    {
        public static IApplicationBuilder UseRequestLimiter(
        this IApplicationBuilder builder,
        int authLimit,
        int anonLimit,
        TimeSpan window)
        {
            return builder.UseMiddleware<RequestLimitingMiddleware>(authLimit, anonLimit, window);
        }
    }
}

