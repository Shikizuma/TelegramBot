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
		
		public FilmModel GetRandomFilm()
		{
			int randomFilm = random.Next(0, Films.Count);
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
				return $"Я не зрозумів, вашої команди: \"{text}\", не існує.";
			}
		}

		public FilmModel[] GetTopFilms(int count)
		{
			return Films.OrderByDescending(f => f.Rate).Take(count).ToArray();
		}
	}
}
