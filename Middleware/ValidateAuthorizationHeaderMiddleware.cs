using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoviesAPI.Services.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading;

namespace MoviesAPI.Middleware
{
    public class ValidateAuthorizationHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateAuthorizationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Authorization"];

            if(!string.IsNullOrEmpty(token))
            {
                var isTokenValidated = await ValidateToken(token);

                if(isTokenValidated == "true")
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return;
                    //context.Response.WriteAsync("Unauthorized", Encoding.UTF8);
                }
            }
            else
            {
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