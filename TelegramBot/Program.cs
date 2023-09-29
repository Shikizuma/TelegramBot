using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Parsers;

namespace TelegramBot
{
	internal class Program
	{
		static string Token = GetJson("config.json");
		
		static void Main(string[] args)
		{
			//var questions = app.GetQuestions();
			var films = MovieParser.ParseMovies();

            MovieBot bot = new MovieBot(Token);
			//bot.Questions = questions;
			bot.Films = films;
			bot.Start();
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