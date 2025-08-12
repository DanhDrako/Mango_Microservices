using log4net;
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Mango.Services.RewardAPI.Service.IService;

namespace Mango.Services.RewardAPI.Service
{
    public class RewardService : IRewardService
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOrderService _orderService;

        public RewardService(IServiceScopeFactory scopeFactory, IOrderService orderService)
        {
            _scopeFactory = scopeFactory;
            _orderService = orderService;
        }

        public async Task UpdateRewards(PaymentQueueDto paymentQueueDto)
        {
            try
            {
                _logger.Info("Process UpdateRewards start.");
                var existingOrder = await _orderService.GetOrder(paymentQueueDto.PaymentIntentId)
                    ?? throw new Exception("Failed to get existing order for updating rewards.");

                // new Reward object creation
                Reward reward = new()
                {
                    OrderId = existingOrder.OrderHeaderId,
                    RewardsActivity = (int)paymentQueueDto.Total,
                    UserId = existingOrder.UserId,
                    RewardsDate = DateTime.Now
                };

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await db.Rewards.AddAsync(reward);
                await db.SaveChangesAsync();

                _logger.Info("[success] Process UpdateRewards end.");
            }
            catch (Exception ex)
            {
                _logger.Error("[fail] Process UpdateRewards end.", ex);
                throw new Exception("Error occurred while updating rewards.", ex);
            }
        }
    }
}
