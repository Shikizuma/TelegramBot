using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
	internal class Program
	{
		static string Token = GetJson("config.json");
		static TelegramBotClient botClient = new TelegramBotClient(Token);
		static void Main(string[] args)
		{
            CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken cancellation = source.Token;

			ReceiverOptions options = new ReceiverOptions()
			{
				AllowedUpdates = {},
			};

			botClient.StartReceiving(BotTakeMessage, BotTakeError, options, cancellation);

			Console.ReadKey();
        }

		static async Task BotTakeMessage(ITelegramBotClient botClient, Update update, CancellationToken token)
		{
			if(update.Type == UpdateType.Message)
			{
				Message message = update.Message;
				if (message.Type == MessageType.Text)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"Привіт, {message.Chat.FirstName}. Я працюю!");
                    await Console.Out.WriteLineAsync(message.Chat.FirstName + ": " + message.Text);
                }
			
			}
			
		}

		static async Task BotTakeError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
		{

		}

		#region Get Json
		static string GetJson(string filePath)
		{
			string key = string.Empty;

			try
			{
				string jsonString = System.IO.File.ReadAllText(filePath);
				JObject json = JObject.Parse(jsonString);
				key = (string)json["key"]!;
				return key;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Помилка при зчитуванні JSON файлу: {ex.Message}");
				return "";
			}

		}
		#endregion
	}
}