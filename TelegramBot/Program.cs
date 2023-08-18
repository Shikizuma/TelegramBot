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
		
		static void Main(string[] args)
		{
			string excelFile = Path.Combine(Environment.CurrentDirectory, "base.xlsx");
			ExcelApp app = new ExcelApp(excelFile);
			var questions = app.GetQuestions();
			var films = app.GetFilms();

            MovieBot bot = new MovieBot(Token);
			bot.Questions = questions;
			bot.Films = films;
			bot.StatisticApp = app;
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