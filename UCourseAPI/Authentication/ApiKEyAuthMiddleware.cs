namespace UCourseAPI.Authentication
{
    public class ApiKEyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKEyAuthMiddleware(RequestDelegate next,IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration; 
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKEyHeaderName, out var extractedapikey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key missing.");
                return;
            }
            var apikey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (apikey != extractedapikey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key missing.");
                return;
            }
            await _next(context);
        }
    }
}
