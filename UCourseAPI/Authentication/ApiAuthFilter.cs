using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UCourseAPI.Authentication
{
    public class ApiAuthFilter : IAuthorizationFilter
    {
        public readonly IConfiguration _configuration;
        public ApiAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKEyHeaderName, out var extractedapikey))
            {
                context.Result = new UnauthorizedObjectResult("API Key Missing.");
                return;
            }
            var apikey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (apikey != extractedapikey)
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key.");
                return;
            }
      
        }
    }
}
