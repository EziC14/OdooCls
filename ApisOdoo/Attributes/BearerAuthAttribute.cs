using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OdooCls.API.Attributes
{
    public class BearerAuthAttribute : Attribute, IAuthorizationFilter
    {
        private const string TokenEsperado = $@"n/KbzsjflJp/wJI+t17W6pqm2cYQMpQe9LZbjWwD5S3zEYkM0zUf3tFsLmScpZG1le6gVsqDA3Qy8VFBdO4aAQ";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult("Falta el token Bearer");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (token != TokenEsperado)
            {
                context.Result = new UnauthorizedObjectResult("Token inválido");
            }
        }
    }
}
