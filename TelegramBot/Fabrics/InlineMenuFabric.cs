using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Enums;

namespace TelegramBot.Fabrics
{
    internal class InlineMenuFabric
    {
        public static InlineKeyboardButton CreateKey(string label)
        {
            return new InlineKeyboardButton(label);
        }

        public static InlineKeyboardButton[][] CreateKeys(MenuType menu)
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
                    menuItems[i][j] = CreateKey(menuItem);
                }
            }

            return menuItems;
        }

    }
}
