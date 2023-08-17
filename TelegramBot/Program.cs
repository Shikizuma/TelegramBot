using Newtonsoft.Json.Linq;

namespace TelegramBot
{
	internal class Program
	{
		static string Token = GetJson("config.json");
		static void Main(string[] args)
		{
            Console.WriteLine(Token);
        }

		static string GetJson(string filePath)
		{
			string key = string.Empty;

			try
			{
				string jsonString = File.ReadAllText(filePath);
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
	}
}