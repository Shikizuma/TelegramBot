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
	partial class MovieBot /* : IMovieBot*/
	{
		static Random random = new Random();
		
		public FilmModel GetRandomFilm()
		{
			int randomFilm = random.Next(0, Films.Count);
			return Films[randomFilm];
		}

		public string GetResponce(string text)
		{
			var question = text.ToLower();
			if (question != null)
			{
				return text;
			}
			else
			{
				return $"Я не зрозумів, вашої команди: \"{text}\", не існує.";
			}
		}

        public List<FilmModel> GetFilmsByGenre(string genre, int count)
        {
            List<FilmModel> filmsWithGenre = Films.Where(f => f.Genres.Contains(genre)).ToList();
            return filmsWithGenre.GetRange(0, count);
        }


        public List<FilmModel> GetTopFilms(int count)
		{
			return Films.OrderByDescending(f => f.RateIMDB).Take(count).ToList();
		}
	}
}
