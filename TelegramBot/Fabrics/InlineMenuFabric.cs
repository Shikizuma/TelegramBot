using Newtonsoft.Json;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Enums;
using TelegramBot.Models;

namespace TelegramBot.Fabrics
{
    internal class InlineMenuFabric
    {
        public static InlineKeyboardButton CreateKey(FilmModel film, string label)
        {
            string callbackData = "";

            switch (label)
            {
                case "Дуже гарно! [⭐️⭐️⭐️⭐️⭐️]":
                    callbackData = film.Id + "|5";
                    break;
                case "Гарно [⭐️⭐️⭐️⭐️]":
                    callbackData = film.Id + "|4";
                    break;
                case "Нормально [⭐️⭐️⭐️]":
                    callbackData = film.Id + "|3";
                    break;
                case "Погано [⭐️⭐️]":
                    callbackData = film.Id + "|2";
                    break;
                case "Дуже погано [⭐️]":
                    callbackData = film.Id + "|1";
                    break;
                case "Ланка на фільм":
                    callbackData = film.Id.ToString();
                    break;
            }
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes("id" + callbackData); 
            if (byteData.Length > 64)
            {
                Console.WriteLine($"Warning: callbackData exceeds the limit of 64 bytes: {byteData.Length} bytes. Data: {callbackData}");
            }
            if (label == "Ланка на фільм")
            {
               
                Console.WriteLine("id" + callbackData);
                return new InlineKeyboardButton(label)
                {
                    CallbackData = "id" + callbackData,
                };
            }
            return new InlineKeyboardButton(label)
            {            
                CallbackData = callbackData
            };
        }

        public static InlineKeyboardButton[][] CreateKeys(FilmModel film, MenuType menu)
        {
            string json = File.ReadAllText("configInlineMenu.json");
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string[][]>>(json);

            string[][] searchKeys = jsonObject[menu.ToString()];
            InlineKeyboardButton[][] menuItems = new InlineKeyboardButton[searchKeys.Length][];

            for (int i = 0; i < searchKeys.Length; i++)
            {
                menuItems[i] = new InlineKeyboardButton[searchKeys[i].Length];

                for (int j = 0; j < searchKeys[i].Length; j++)
                {
                    string menuItem = searchKeys[i][j];
                    menuItems[i][j] = CreateKey(film, menuItem);
                }
            }

            return menuItems;
        }
    }
}
