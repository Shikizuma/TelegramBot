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
                case "Дуже гарно! (⭐️⭐️⭐️⭐️⭐️)":
                    callbackData = film.Name + "|5";
                    break;
                case "Непогано (⭐️⭐️⭐️⭐️)":
                    callbackData = film.Name + "|4";
                    break;
                case "Нормально (⭐️⭐️⭐️)":
                    callbackData = film.Name + "|3";
                    break;
                case "Погано (⭐️⭐️)":
                    callbackData = film.Name + "|2";
                    break;
                case "Дуже погано (⭐️)":
                    callbackData = film.Name + "|1";
                    break;
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
