using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoviesAPI.Services.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace MoviesAPI.Middleware
{
    public class ValidateAuthorizationHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<ValidateAuthorizationHeaderMiddleware> _logger;
        public ValidateAuthorizationHeaderMiddleware(RequestDelegate next, ILogger<ValidateAuthorizationHeaderMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Authorization"];
            var _agent = context.Request.Headers[HeaderNames.UserAgent].ToString();
            var _requestPath = context.Request.Path;
            
            if(!string.IsNullOrEmpty(token))
            {
                var isTokenValidated = await ValidateToken(token);

                if(isTokenValidated == "true")
                {
                    _logger.LogInformation($"JWT token validated SUCCESSFULLY for request {_requestPath} from browser {_agent} at {DateTime.UtcNow.ToLongTimeString()}");
                    await _next(context);
                }
                else
                {
                    _logger.LogError($"JWT token validation FAILED for request {_requestPath} from browser {_agent} at {DateTime.UtcNow.ToLongTimeString()}");
                    context.Response.StatusCode = 401;
                    return;
                    //context.Response.WriteAsync("Unauthorized", Encoding.UTF8);
                }
            }
            else
            {
                _logger.LogError($"EMPTY JWT token found for request {_requestPath} from browser {_agent} at {DateTime.UtcNow.ToLongTimeString()}");
                context.Response.StatusCode = 401;
                return;
            }
            await _next(context);
        }
        
        private async Task<string> ValidateToken(string token)
        {
            try
            {
                AuthorizeAPI authorizeAPI = new AuthorizeAPI();
                HttpClient client = authorizeAPI.InitializeClient();
                client.DefaultRequestHeaders.Add("token", token);
                HttpResponseMessage response = await client.GetAsync("api/authorize/validatetoken");
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch
            {
                return "false";
            }
        }
    } 
}