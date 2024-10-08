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
            await _adapter.ProcessAsync(Request, Response, _bot);
        }

    }

}
