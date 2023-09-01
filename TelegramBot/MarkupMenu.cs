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
				return new ReplyKeyboardMarkup(MenuFabric.CreateKeys(MenuType.MainMenu));
			}
		}
		public static IReplyMarkup SearchMenu 
		{
			get
			{
				return new ReplyKeyboardMarkup(MenuFabric.CreateKeys(MenuType.SearchMenu));
			}
		}

		public static IReplyMarkup GenreMenu
		{
			get
			{
				return new ReplyKeyboardMarkup(MenuFabric.CreateKeys(MenuType.GenreMenu));
			}
		}

	}
}
