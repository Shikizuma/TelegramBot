using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
	internal class MarkupMenu
	{
		public static IReplyMarkup MainMenu 
		{
			get
			{
				KeyboardButton[][] buttons = new KeyboardButton[][]
				{
					new KeyboardButton[] {new KeyboardButton("Рандомний фільм"), new KeyboardButton("Топ фільмів")},
					new KeyboardButton[] {new KeyboardButton("Знайти фільм")},
					new KeyboardButton[] {new KeyboardButton("Статистика")},
				};
				return new ReplyKeyboardMarkup(buttons);
			}
		}
		public static IReplyMarkup SearchMenu 
		{
			get
			{
				KeyboardButton[][] buttons = new KeyboardButton[][]
				{
					new KeyboardButton[] {new KeyboardButton("За жанром"), new KeyboardButton("За назвою")},
					new KeyboardButton[] {new KeyboardButton("Назад у меню")},
				};
				return new ReplyKeyboardMarkup(buttons);
			}
		}

		public static IReplyMarkup GenreMenu
		{
			get
			{
				KeyboardButton[][] buttons = new KeyboardButton[][]
				{
					new KeyboardButton[] {new KeyboardButton("Драма"), new KeyboardButton("Комедія"), new KeyboardButton("Екшн"), new KeyboardButton("Жахи") },
					new KeyboardButton[] {new KeyboardButton("Фантастика"), new KeyboardButton("Фентезі"), new KeyboardButton("Містика"), new KeyboardButton("Анімація") },
					new KeyboardButton[] {new KeyboardButton("Трилер"), new KeyboardButton("Романтика"), new KeyboardButton("Музичний"), new KeyboardButton("Спорт") },
					new KeyboardButton[] {new KeyboardButton("Документальний фільм"), new KeyboardButton("Історична драма"), new KeyboardButton("Кримінальна драма") },
					new KeyboardButton[] {new KeyboardButton("Назад у меню") },
				};
				return new ReplyKeyboardMarkup(buttons);
			}
		}

	}
}
