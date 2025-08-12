using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.RewardAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;


        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _accessor = accessor;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = null;

            if (_accessor.HttpContext != null)
            {
                token = await _accessor.HttpContext.GetTokenAsync("access_token");
            }
            else
            {
                token = _configuration["InternalServiceToken"]; // e.g., from appsettings.json or env
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
