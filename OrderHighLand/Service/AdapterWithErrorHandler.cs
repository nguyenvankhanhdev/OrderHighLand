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
                // Ghi log chi tiết lỗi
                logger.LogError($"Exception caught : {exception.Message}");
                Console.WriteLine($"Error: {exception.Message}");
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
