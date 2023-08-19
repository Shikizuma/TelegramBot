using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Enums;
using TelegramBot.Models;

namespace TelegramBot.Interface
{
	internal interface IMovieBot
	{
		FilmModel GetRandomFilm();
		FilmModel[] GetTopFilms(int count);
		//FilmModel[] GetByTags(string search, SearchMode mode);
		string GetStatisticViews();
		string GetResponce(string question);
	}
}
