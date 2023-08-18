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

namespace TelegramBot
{
	partial class MovieBot
	{
		TelegramBotClient botClient;
		public QuestionModel[] Questions { private get; set; }
		public FilmModel[] Films { private get; set; }
		public MovieBot(string token)
        {
			botClient = new TelegramBotClient(token);		
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

		public void SetQuestions(QuestionModel[] questions)
		{

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
		}

		public async Task GetTextMessage(Message message)
		{
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

				if (message.Text == "За жанром")
				{

				}
				if (message.Text == "За назвою")
				{

				}
				if (message.Text == "Назад у меню")
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, "Ви повернулись у головне меню", replyMarkup: MarkupMenu.MainMenu);
				}
				return;
			}

			string responce = GetResponce(message.Text);
			await botClient.SendTextMessageAsync(message.Chat.Id, responce, replyMarkup: MarkupMenu.MainMenu);
			await Console.Out.WriteLineAsync(message.Chat.FirstName + ": " + message.Text);
		}


		public async Task ShowFilm(long chat, FilmModel film)
		{
			var buttons = InlineMenu.SetRate(film);
			await botClient.SendPhotoAsync(
				chatId: chat, 
				photo: film.Image, 
				caption: film.Name + "\n" + film.Description + "\n" + "Рейтинг: " + film.Rate, 
				replyMarkup: buttons);
			//await botClient.SendTextMessageAsync(chatId: chat, );

		}

		async Task BotTakeError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
		{

		}
	}
}
