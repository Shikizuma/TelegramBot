﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Enums;
using TelegramBot.Models;

namespace TelegramBot.Interface
{
	internal interface IMovieBot
	{
		Film GetRandomFilm();
		Film[] GetTopFilms(int count);
		Film[] GetByTags(string search, SearchMode mode);
		string GetStatisticViews();
		string GetResponce(string question);
	}
}
