using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Enums;

namespace TelegramBot.Fabrics
{
	internal class MenuFabric
	{
		public static KeyboardButton CreateKey(string label)
		{
			return new KeyboardButton(label);
		}

		public static KeyboardButton[][] CreateKeys(MenuType menu)
		{
			string json = File.ReadAllText("configMenu.json");
			var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string[][]>>(json);

			string[][] searchKeys = jsonObject[menu.ToString()];
			KeyboardButton[][] menuItems = new KeyboardButton[searchKeys.Length][];

			for (int i = 0; i < searchKeys.Length; i++)
			{
				menuItems[i] = new KeyboardButton[searchKeys[i].Length];

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
