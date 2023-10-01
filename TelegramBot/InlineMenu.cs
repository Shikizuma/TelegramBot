using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Fabrics;
using TelegramBot.Models;

namespace TelegramBot
{
	internal class InlineMenu
	{
		public static IReplyMarkup SetRate(FilmModel film)
		{	
			return new InlineKeyboardMarkup(InlineMenuFabric.CreateKeys(film, Enums.MenuType.GradesInlineMenu));
		}
	}
}
