using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace OrderHighLand.Service
{
    public class ChatbotService : ActivityHandler
    {
        private readonly ProductService _productService;
        public ChatbotService(ProductService productService)
        {
            _productService = productService;
        }
        // Hàm xử lý tin nhắn từ người dùng và phản hồi
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(turnContext.Activity.ServiceUrl))
            {
                // Log lỗi hoặc gửi phản hồi thông báo ServiceUrl không hợp lệ
                Console.WriteLine("ServiceUrl is null or empty.");
                throw new ArgumentNullException(nameof(turnContext.Activity.ServiceUrl), "ServiceUrl cannot be null or empty.");
            }
            try
            {
                string userMessage = turnContext.Activity.Text.ToLower().Trim(); 

                if (userMessage.Contains("giá"))
                {
                    var productNames = await _productService.GetAllProductNamesAsync();

                    string productName = ExtractProductName(userMessage, productNames);
                    string size = ExtractSize(userMessage);

                    Console.WriteLine($"Sản phẩm: {productName}, Kích thước: {size}");

                    if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(size))
                    {
                        string price = await _productService.GetProductPriceAsync(productName, size);

                        if (!string.IsNullOrEmpty(price))
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text(price), cancellationToken);
                        }
                        else
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("Không tìm thấy thông tin giá cho sản phẩm này."), cancellationToken);
                        }
                    }
                    else
                    {
                        string missingInfo = string.IsNullOrEmpty(productName) ? "tên sản phẩm" : "kích thước";
                        await turnContext.SendActivityAsync(MessageFactory.Text($"Xin vui lòng cung cấp {missingInfo} mà bạn muốn hỏi giá."), cancellationToken);
                    }
                }
                else
                {
                    var replyText = $"Bạn vừa nói: {turnContext.Activity.Text}";
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn."), cancellationToken);
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        // Hàm tính khoảng cách Levenshtein
        private int LevenshteinDistance(string source, string target)
        {
            var d = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= target.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[source.Length, target.Length];
        }


        private string ExtractProductName(string message, List<string> productNames)
        {
            var messageWords = message.Split(' ');

            string bestMatch = string.Empty;
            int minDistance = int.MaxValue;

            foreach (var productName in productNames)
            {
                var productWords = productName.ToLower().Split(' ');

                foreach (var word in messageWords)
                {
                    foreach (var productWord in productWords)
                    {
                        int distance = LevenshteinDistance(word, productWord); 

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bestMatch = productName; 
                        }
                    }
                }
            }

        
            if (minDistance <= 2) 
            {
                return bestMatch;
            }

            return string.Empty;
        }


        // Hàm giả định để tách kích thước từ câu hỏi
        private string ExtractSize(string message)
        {
            if (message.Contains("nhỏ"))
            {
                return "Nhỏ";
            }
            else if (message.Contains("vừa"))
            {
                return "Vừa";
            }
            else if (message.Contains("lớn"))
            {
                return "Lớn";
            }
            return string.Empty;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Xin chào! Tôi là bot của bạn!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }

}
