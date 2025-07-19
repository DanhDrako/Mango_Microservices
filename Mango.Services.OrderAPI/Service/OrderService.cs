using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Service
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public OrderService(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<OrderHeaderDto> CreateOrder(CartHeaderDto cartHeader)
        {
            OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartHeader);
            orderHeaderDto.OrderTime = DateTime.Now;
            orderHeaderDto.Status = SD.Status_Pending;
            orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartHeader.CartDetails);
            orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);

            OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

            await _db.SaveChangesAsync();

            orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
            return orderHeaderDto;
        }

        public async Task<OrderHeaderDto> GetOrderById(int orderHeaderId)
        {
            OrderHeader orderHeader = await _db.OrderHeaders
                .Include(o => o.OrderDetails)
                .FirstAsync(o => o.OrderHeaderId == orderHeaderId);

            return _mapper.Map<OrderHeaderDto>(orderHeader);
        }

        public async Task<IEnumerable<OrderHeaderDto>> GetOrdersByUserId(string? userId, bool isAdmin)
        {
            IEnumerable<OrderHeader> objList;
            if (isAdmin)
            {
                objList = await _db.OrderHeaders
                    .Include(o => o.OrderDetails)
                    .OrderByDescending(u => u.OrderHeaderId).ToListAsync();
            }
            else
            {
                objList = await _db.OrderHeaders
                    .Include(o => o.OrderDetails)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(u => u.OrderHeaderId).ToListAsync();
            }

            return _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
        }

        public Task<OrderHeaderDto> UpdateOrderStatus(int orderHeaderId, string status)
        {
            throw new NotImplementedException();
        }
    }
}
