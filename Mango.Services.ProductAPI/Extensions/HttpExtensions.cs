using Mango.Services.ProductAPI.RequestHelpers;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace Mango.Services.ProductAPI.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeaders(this HttpResponse response, PaginationMetadata metadata)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(metadata, options));
            response.Headers.Append(HeaderNames.AccessControlExposeHeaders, "Pagination");
        }
    }
}
