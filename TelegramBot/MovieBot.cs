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
					if(message.Text == "Рандомний фільм")
					{
						var film = GetRandomFilm();
						await ShowFilm(message.Chat.Id, film);
						return;
					}

					if (message.Text == "Топ фільмів")
					{
						var films = GetTopFilms(5);
						foreach(var film in films)
						{
							await ShowFilm(message.Chat.Id, film);
						}
						return;
					}

					string responce = GetResponce(message.Text);
					await botClient.SendTextMessageAsync(message.Chat.Id, responce);
					await Console.Out.WriteLineAsync(message.Chat.FirstName + ": " + message.Text);
				}		
			}
		}
		
		public async Task ShowFilm(long chat, FilmModel film)
		{
			await botClient.SendPhotoAsync(chatId: chat, photo: film.Image);
			await botClient.SendTextMessageAsync(chatId: chat, film.Name +  "\n" + film.Description);
			await botClient.SendTextMessageAsync(chatId: chat,  "Рейтинг: " + film.Rate);

		}

		async Task BotTakeError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
		{

		}
	}
}
