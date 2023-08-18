using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Enums;
using TelegramBot.Interface;
using TelegramBot.Models;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
	partial class MovieBot : IMovieBot
	{
		static Random random = new Random();
		public FilmModel[] GetByTags(string search, SearchMode mode)
		{
			throw new NotImplementedException();
		}

		public FilmModel GetRandomFilm()
		{
			int randomFilm = random.Next(0, Films.Length);
			return Films[randomFilm];
		}

		public string GetResponce(string text)
		{
			var question = Questions.FirstOrDefault(q => q.Question.Contains(text.ToLower()));
			if (question != null)
			{
				return question.Responce;
			}
			else
			{
				return $"Що? ЩО? Я НЕ ЗРОЗУМІВ НА ЦЬОМУ МОМЕНТІ: \"{text}\"";
			}
		}

		public string GetStatisticViews()
		{
			return StatisticApp.GetStatistics(Films);
		}

		public FilmModel[] GetTopFilms(int count)
		{
			return Films.OrderByDescending(f => f.Rate).Take(count).ToArray();
		}
	}
}
