using Mango.Services.PaymentAPI.Models.Dto;
using Mango.Services.PaymentAPI.Models.Dto.Order;
using Mango.Services.PaymentAPI.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.PaymentAPI.Service
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        public async Task<OrderHeaderDto> GetOrder(int orderHeaderId)
        {
            var client = _httpClientFactory.CreateClient("Order");
            var response = await client.GetAsync($"/api/order/{orderHeaderId}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(resp.Result));
            }
            return null;
        }

        public async Task<OrderHeaderDto> UpdateHeaderDto(OrderHeaderDto orderHeaderDto)
        {
            var client = _httpClientFactory.CreateClient("Order");

            // Serialize the OrderHeaderDto to JSON
            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(orderHeaderDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"/api/order", jsonContent);
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
