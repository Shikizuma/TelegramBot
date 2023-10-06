using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Models;

namespace TelegramBot.Handlers
{
 

    internal class MessageHandler
    {

        TelegramBotClient botClient;
        public List<FilmModel> Films { private get; set; }
        Dictionary<long, string> Context { get; set; }
        private readonly MovieBot movieBot;

        public MessageHandler(List<FilmModel> films, TelegramBotClient botClient, Dictionary<long, string> context)
        {
            Films = films;
            this.botClient = botClient;
            Context = context;
        }


        public async Task HandleUserMessage(Message message)
        {
            if (Context.ContainsKey(message.Chat.Id))
            {
                string _context = Context[message.Chat.Id];
                Context[message.Chat.Id] = message.Text;

                await HandleContextSpecificActions(message, _context);
            }
            else
            {
                Context.Add(message.Chat.Id, message.Text);
                await HandleInitialActions(message);
            }
        }

        private async Task HandleContextSpecificActions(Message message, string context)
        {
            switch (context)
            {
                case "За жанром":
                case "За назвою":
                    await HandleFilmRequest(message, context);
                    break;
                default:
                    await HandleNonFilmRequest(message);
                    break;
            }
        }

        private async Task HandleFilmRequest(Message message, string context)
        {
            string searchTerm = message.Text;
            List<FilmModel> filmRequest = null;
            int count = 0;

            if (context == "За жанром")
            {
                filmRequest = movieBot.GetFilmsByGenre(searchTerm);
            }
            else if (context == "За назвою")
            {
                filmRequest = Films.Where(f => f.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }

            count = filmRequest.Count;

            if (count > 0)
            {
                if (context == "За жанром")
                {
                    int random = new Random().Next(0, count);
                    filmRequest = new List<FilmModel> { filmRequest[random] };
                }

                foreach (var film in filmRequest)
                {
                    await movieBot.ShowFilm(message.Chat.Id, film);
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Всього знайдено фільмів: {count}", replyMarkup: MarkupMenu.MainMenu);
            }
            else
            {
                string errorMessage = (context == "За жанром") ?
                    $"Не вдалось знайти фільм по цьому жанру {searchTerm}." :
                    $"Не вдалось знайти фільм по цій назві {searchTerm}.";

                await botClient.SendTextMessageAsync(message.Chat.Id, errorMessage, replyMarkup: MarkupMenu.MainMenu);
            }
        }


        private async Task HandleInitialActions(Message message)
        {
            switch (message.Text)
            {
                case "За жанром":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть жанр", replyMarkup: MarkupMenu.GenreMenu);
                    break;
                case "За назвою":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть назву фільма", replyMarkup: MarkupMenu.SearchMenu);
                    break;
                case "Рандомний фільм":
                    var film = movieBot.GetRandomFilm();
                    await movieBot.ShowFilm(message.Chat.Id, film);
                    break;
                case "Топ фільмів":
                    var films = movieBot.GetTopFilms(5);
                    foreach (var topFilm in films)
                    {
                        await movieBot.ShowFilm(message.Chat.Id, topFilm);
                    }
                    break;
                case "Знайти фільм":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть тип пошуку", replyMarkup: MarkupMenu.SearchMenu);
                    break;
                case "Назад у меню":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ви повернулись у головне меню", replyMarkup: MarkupMenu.MainMenu);
                    break;
                default:
                    string response = movieBot.GetResponce(message.Text);
                    await botClient.SendTextMessageAsync(message.Chat.Id, response, replyMarkup: MarkupMenu.MainMenu);
                    break;
            }
        }

        private async Task HandleNonFilmRequest(Message message)
        {
            string response = movieBot.GetResponce(message.Text);
            await botClient.SendTextMessageAsync(message.Chat.Id, response, replyMarkup: MarkupMenu.MainMenu);
        }
    }
}
