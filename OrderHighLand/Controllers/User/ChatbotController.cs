using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;

namespace OrderHighLand.Controllers.User
{
    [Route("api/messages")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IBot _bot;
        private readonly IBotFrameworkHttpAdapter _adapter;

        public ChatbotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            try
            {
                Console.WriteLine("Received a POST request at /api/messages");
                await _adapter.ProcessAsync(Request, Response, _bot);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                Console.WriteLine($"Error processing the request: {ex.Message}");
                Console.WriteLine(ex.StackTrace); // Thêm log chi tiết StackTrace để biết nguyên nhân
                Response.StatusCode = 500;
                await Response.WriteAsync("Error processing the request.");
            }
        }

    }

}
