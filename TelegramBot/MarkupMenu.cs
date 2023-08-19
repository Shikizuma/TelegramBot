using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Enums;
using TelegramBot.Fabrics;

namespace TelegramBot
{
	internal class MarkupMenu
	{
		public static IReplyMarkup MainMenu 
		{
			get
			{
				KeyboardButton[][] buttons = MenuFabric.CreateKeys(MenuType.MainMenu);
				return new ReplyKeyboardMarkup(buttons);
			}
		}
		public static IReplyMarkup SearchMenu 
		{
			get
			{
				KeyboardButton[][] buttons = MenuFabric.CreateKeys(MenuType.SearchMenu);
				return new ReplyKeyboardMarkup(buttons);
			}
		}

		public static IReplyMarkup GenreMenu
		{
			get
			{
				KeyboardButton[][] buttons = MenuFabric.CreateKeys(MenuType.GenreMenu);
				return new ReplyKeyboardMarkup(buttons);
			}
		}

	}
}
