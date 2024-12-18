﻿using System;
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
using Newtonsoft.Json;
using Telegram.Bot.Types.InlineQueryResults;
using TelegramBot.Handlers;

namespace TelegramBot
{
    internal partial class MovieBot
	{
		TelegramBotClient botClient;
		public QuestionModel[] Questions { private get; set; }
		public List<FilmModel> Films { private get; set; }
		Dictionary<long, string> Context { get; set; }
        MessageHandler messageHandler { get; set; }

        string jsonFilePath = "movies.json";

        public MovieBot(string token)
        {
			botClient = new TelegramBotClient(token);	
			Context = new Dictionary<long, string>();
			messageHandler = new(Films, botClient, Context);

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
				await GetCallBack(update.CallbackQuery);
			}
		}

		public async Task GetCallBack(CallbackQuery callback)
		{
			if (callback.Data == null)
				return;

            if (callback.Data.StartsWith("id"))
            {
				int filmID = int.Parse(callback.Data.Replace("id", ""));
				FilmModel film = Films.Where(f => f.Id == filmID).First();
				Console.WriteLine(filmID);
                await botClient.SendTextMessageAsync(callback.From.Id, $"Перейдіть за посиланням:{film.MovieUrl}");
            }
			else
            {
                string[] messages = callback.Data.Split('|');

                int filmId = int.Parse(messages[0]);
                int rate = Int32.Parse(messages[1]);

                var film = Films.FirstOrDefault(f => f.Id == filmId);
                if (film != null)
                {
                    double currentViews = film.Views++;
                    double rating = (currentViews * film.Rate + rate) / film.Views;
                    film.Rate = rating;

                    SaveFilmsToJson();

                    await botClient.SendTextMessageAsync(callback.From.Id, "🤗Дякуємо за вашу оцінку! Рейтинг цього фільма став: " + film.Rate);
                }
            }               
		}

		public async Task GetTextMessage(Message message)
		{
            await messageHandler.HandleUserMessage(message);
            //if (Context.ContainsKey(message.Chat.Id))
            //{
            //	string context = Context[message.Chat.Id];
            //	Context[message.Chat.Id] = message.Text;
            //	if (context == "За жанром" || context == "За назвою")
            //	{
            //		await HandleFilmRequest(message, context);
            //		return;
            //	}
            //}
            //else
            //{
            //	Context.Add(message.Chat.Id, message.Text);
            //}
            //if (message.Text == "За жанром")
            //{
            //	await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть жанр", replyMarkup: MarkupMenu.GenreMenu);
            //	return;
            //}
            //if (message.Text == "За назвою")
            //{
            //	await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву фільма", replyMarkup: MarkupMenu.SearchMenu);
            //	return;
            //}
            //if (message.Text == "Рандомний фільм")
            //{
            //	var film = GetRandomFilm();
            //	await ShowFilm(message.Chat.Id, film);
            //	return;
            //}
            //if (message.Text == "Топ фільмів")
            //{
            //	var films = GetTopFilms(5);
            //	foreach (var film in films)
            //	{
            //		await ShowFilm(message.Chat.Id, film);
            //	}
            //	return;
            //}
            //if (message.Text == "Знайти фільм")
            //{
            //	await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть тип пошуку", replyMarkup: MarkupMenu.SearchMenu);
            //	return;
            //}
            //if (message.Text == "Назад у меню")
            //{
            //	await botClient.SendTextMessageAsync(message.Chat.Id, "Ви повернулись у головне меню", replyMarkup: MarkupMenu.MainMenu);
            //	return;
            //}
            //string responce = GetResponce(message.Text);
            //await botClient.SendTextMessageAsync(message.Chat.Id, responce, replyMarkup: MarkupMenu.MainMenu);
        }

     

        public async Task ShowFilm(long chat, FilmModel film)
		{
			var buttons = InlineMenu.SetRate(film);

			try
			{
                Console.WriteLine(film.Image);
                await botClient.SendPhotoAsync(
					chatId: chat,
					photo: film.Image,
					caption: film.ToString(),
					replyMarkup: buttons);
			}
			catch (Exception ex)
			{
                Console.WriteLine("Error: " + ex.HelpLink + ex.Message);
            }
		}

		async Task BotTakeError(ITelegramBotClient botClient, Exception ex, CancellationToken token)
		{
		}

        private void SaveFilmsToJson()
        {
            using (StreamWriter writer = new StreamWriter(jsonFilePath))
            {
                string json = JsonConvert.SerializeObject(Films, Formatting.Indented);
                writer.Write(json);
            }
        }
    }
}
