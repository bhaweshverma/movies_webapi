using System.Net.Http;
using System;
using System.Net.Http.Headers;

namespace MoviesAPI.Services.Helper
{
    public class AuthorizeAPI
    {
        private string _authorizeApiBaseUri = "http://localhost:5003";
        public HttpClient InitializeClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_authorizeApiBaseUri);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}