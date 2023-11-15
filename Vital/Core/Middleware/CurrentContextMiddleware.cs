using Vital.Core.Context;

namespace Vital.Core.Middleware;

public class CurrentContextMiddleware{
    private readonly RequestDelegate _next;

    public CurrentContextMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, CurrentContext currentContext) {
        currentContext.Build(httpContext);
        await _next.Invoke(httpContext);
    }
}
