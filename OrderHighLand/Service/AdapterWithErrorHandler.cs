using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace OrderHighLand.Service
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
           : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log lỗi khi có exception xảy ra
                logger.LogError($"Exception caught : {exception.Message}");

                // Gửi thông báo lỗi đến người dùng
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
