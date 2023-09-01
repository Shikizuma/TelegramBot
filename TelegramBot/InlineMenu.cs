using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot
{
	internal class InlineMenu
	{
		public static IReplyMarkup SetRate(FilmModel film)
		{
			InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][]
			{
				new InlineKeyboardButton[]
				{
					new InlineKeyboardButton("Вражаюче")
					{
						CallbackData = film.Name + "|5"
					},
					new InlineKeyboardButton("Гарно")
					{
						CallbackData = film.Name + "|4"
					},
				},
				new InlineKeyboardButton[]
				{
					new InlineKeyboardButton("Середньо")
					{
						CallbackData = film.Name + "|3"
					},
					new InlineKeyboardButton("Погано")
					{
						CallbackData = film.Name + "|2"
					},
				},
				new InlineKeyboardButton[]
				{
					new InlineKeyboardButton("Жахливо")
					{
						CallbackData = film.Name + "|1"
					},
				},
			};

			return new InlineKeyboardMarkup(buttons);
		}
	}
}
