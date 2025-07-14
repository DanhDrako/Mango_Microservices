using log4net;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto.Cart;
using Mango.Services.EmailAPI.Service.IService;
using System.Text;

namespace Mango.Services.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceScopeFactory _scopeFactory;
        public EmailService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Logs the cart details and sends an email to the specified address with the cart information.
        /// </summary>
        /// <param name="cartDto"></param>
        /// <returns></returns>
        public async Task EmailCartAndLog(CartHeaderDto cartDto)
        {
            if (cartDto == null || string.IsNullOrEmpty(cartDto.Email))
            {
                _logger.Error("Invalid cartDto or email provided for EmailCartAndLog.");
                return;
            }

            StringBuilder message = new();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Total " + cartDto.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");

            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product?.Name + " x " + item.Quantity);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.Email);
            _logger.Info($"EmailCartAndLog successfully for userId: {cartDto.UserId}, userEmail: {cartDto.Email}, cartHeaderId: {cartDto.CartHeaderId}");
        }

        /// <summary>
        /// Logs the user registration email and sends a confirmation email to the specified address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task EmailRegisterUserAndLog(string email)
        {
            string message = "User Registeration Successful. <br/> Email : " + email;
            await LogAndEmail(message, "nkocdanh58@gmail.com");
            _logger.Info($"EmailRegisterUserAndLog successfully for userEmail: {email}");
        }

        /// <summary>
        /// Logs the provided message and sends an email to the specified address.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                EmailLogger emailLog = new()
                {
                    Email = email,
                    Message = message
                };
                await db.EmailLoggers.AddAsync(emailLog);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while logging email:", ex);
                return false;
            }

        }
    }
}
