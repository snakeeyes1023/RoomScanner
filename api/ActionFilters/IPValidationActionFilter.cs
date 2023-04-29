using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RoomScannerWeb.ActionFilters
{
    /// <summary>
    /// Permet de valider que l'utilisateur actuelle est une ip valide
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute" />
    public class IPValidationActionFilter : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedIPs;

        public IPValidationActionFilter(IConfiguration configuration)
        {
            _configuration = configuration;
            _allowedIPs = _configuration.GetSection("AllowedApiIPs").Get<string[]>();
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
#if RELEASE
            string remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();

            if (!_allowedIPs.Contains(remoteIpAddress))
            {
                context.Result = new ContentResult
                {
                    Content = "accès refusé",
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
#endif
        }
    }
}
