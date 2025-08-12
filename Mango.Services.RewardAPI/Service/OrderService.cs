using Mango.Services.RewardAPI.Models.Dto;
using Mango.Services.RewardAPI.Models.Dto.Order;
using Mango.Services.RewardAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.RewardAPI.Service
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        public async Task<OrderHeaderDto> GetOrder(string paymentIntentId)
        {
            var client = _httpClientFactory.CreateClient("Order");
            var response = await client.GetAsync($"/api/order/intent/{paymentIntentId}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(resp.Result));
            }
            return null;
        }
    }
}
