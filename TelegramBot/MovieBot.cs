using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Newtonsoft.Json.Linq;
using TelegramBot.Models;
using TelegramBot.Interface;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Parsers;

namespace TelegramBot
{
	partial class MovieBot
	{
		TelegramBotClient botClient;
		public QuestionModel[] Questions { private get; set; }
		public List<FilmModel> Films { private get; set; }
		Dictionary<long, string> Context { get; set; }
        public MovieBot(string token)
        {
			botClient = new TelegramBotClient(token);	
			Context = new Dictionary<long, string>();
		}

		public void Start()
		{
			CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken cancellation = source.Token;

			ReceiverOptions options = new ReceiverOptions()
			{
				AllowedUpdates = { },
			};
	
			botClient.StartReceiving(BotTakeMessage, BotTakeError, options, cancellation);

            Console.WriteLine("Бот почав роботу!");
            Console.ReadKey();
		}

		async Task BotTakeMessage(ITelegramBotClient botClient, Update update, CancellationToken token)
		{
			if (update.Type == UpdateType.Message)
			{
				Message message = update.Message;

				if(message.Type == MessageType.Text)
				{
					GetTextMessage(message);
				}		
			}
			else if(update.Type == UpdateType.CallbackQuery)
			{
				//await GetCallBack(update.CallbackQuery);
			}
		}

		public async Task GetCallBack(CallbackQuery callback)
		{
			//if (callback.Data == null)
			//	return;

			//string[] messages = callback.Data.Split('|');

			//string filmName = messages[0];
			//int rate = Int32.Parse(messages[1]);

			//var film = Films.FirstOrDefault(f => f.Name == filmName);
			//if (film != null)
			//{
			//	double currentViews = film.Views++;
			//	double rating = (currentViews * film.Rate + rate) / film.Views;
			//	film.Rate = rating;
			//	await botClient.SendTextMessageAsync(callback.From.Id, "Рейтинг: " + film.Rate);
			//}
		}

		public async Task GetTextMessage(Message message)
		{

			if (Context.ContainsKey(message.Chat.Id))
			{
				string context = Context[message.Chat.Id];
				Context[message.Chat.Id] = message.Text;

				if (context == "За жанром" || context == "За назвою")
				{
					await HandleFilmRequest(message, context);
					return;
				}
			}
			else
			{
				Context.Add(message.Chat.Id, message.Text);
			}

			if (message.Text == "За жанром")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть жанр", replyMarkup: MarkupMenu.GenreMenu);
				return;
			}
			if (message.Text == "За назвою")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву фільма", replyMarkup: MarkupMenu.SearchMenu);
				return;
			}

			if (message.Text == "Рандомний фільм")
			{
				var film = GetRandomFilm();
				await ShowFilm(message.Chat.Id, film);
				return;
			}

			if (message.Text == "Топ фільмів")
			{
				var films = GetTopFilms(5);
				foreach (var film in films)
				{
					await ShowFilm(message.Chat.Id, film);
				}
				return;
			}

			if (message.Text == "Знайти фільм")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть тип пошуку", replyMarkup: MarkupMenu.SearchMenu);
				return;
			}

			if (message.Text == "Назад у меню")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Ви повернулись у головне меню", replyMarkup: MarkupMenu.MainMenu);
				return;
			}


			string responce = GetResponce(message.Text);
			await botClient.SendTextMessageAsync(message.Chat.Id, responce, replyMarkup: MarkupMenu.MainMenu);
		}

		private async Task HandleFilmRequest(Message message, string context)
		{
			string searchTerm = message.Text.ToLower();
			List<FilmModel> filmRequest = null;

			if (context == "За жанром")
			{
                filmRequest = GetFilmsByGenre(searchTerm, 3);
            }
			else if (context == "За назвою")
			{
				filmRequest = Films.Where(f => f.Name.ToLower().Contains(searchTerm)).ToList();
			}

			if (filmRequest != null && filmRequest.Count > 0)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, $"Всього знайдено фільмів: {filmRequest.Count}.");
				foreach (var film in filmRequest)
				{
					await ShowFilm(message.Chat.Id, film);
				}
			}
			else
			{
				string errorMessage = (context == "За жанром") ?
					$"Не вдалось знайти фільм по цьому жанру {searchTerm}." :
					$"Не вдалось знайти фільм по цій назві {searchTerm}.";

				await botClient.SendTextMessageAsync(message.Chat.Id, errorMessage, replyMarkup: MarkupMenu.MainMenu);
			}
		}

		public async Task ShowFilm(long chat, FilmModel film)
		{
			//var buttons = InlineMenu.SetRate(film);
			try
			{
                Console.WriteLine(film.Image);
                await botClient.SendPhotoAsync(
					chatId: chat,
					photo: film.Image,
					caption: film.ToString()
					/*replyMarkup: buttons*/);
			}
			catch (Exception ex)
			{
                Console.WriteLine("Error: "+ ex.Message);
            }
		}

		async Task BotTakeError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
		{
		}
	}
}
