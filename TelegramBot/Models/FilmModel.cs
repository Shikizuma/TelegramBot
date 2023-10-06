using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Enums;

namespace TelegramBot.Models
{
	internal class FilmModel
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public List<string> Countries { get; set; }
        public Dictionary<string, string> Directors { get; set; }
        public Dictionary<string, string> Actors { get; set; }
        public List<string> Genres { get; set; }
        public string Duration { get; set; }
        public double RateIMDB { get; set; }
        public double Rate { get; set; }
        public int ViewsIMDB { get; set; }
        public int Views { get; set; }
        public string Image { get; set; }
        public string MovieUrl { get; set; }


        public FilmModel()
        {
            Countries = new List<string>();
            Directors = new Dictionary<string, string>();
            Actors = new Dictionary<string, string>();
            Genres = new List<string>();
        }

        public override string ToString()
        {
            string genresString = string.Join(", ", Genres);
            string countriesString = string.Join(", ", Countries);
            string directorString = string.Join(", ", Directors.Values);
            string actorsString = string.Join(", ", Actors.Values);
            return $"🎬 Назва: {Name}\n" +
                 $"🎭 Жанр: {genresString}\n" +
                 $"⏳ Тривалість: {Duration}\n" +
                 $"💬 Опис: {Description}\n" +
                 $"🌍 Країна: {countriesString}\n" +
                 $"📽 Режисер: {directorString}\n" +
                 $"👥 Актори: {actorsString}\n" +
                 $"👀 Перегляди (Користувачів бота): {(Views < 1 ? "Немає переглядів" : Views)}\n" +
                 $"⭐ Рейтинг (Користувачів бота): {(Rate < 1 ? "Немає оцінок" : Rate + " / 5")}\n" +
                 $"👁 Перегляди IMDB: {ViewsIMDB}\n" +
                 $"👑 Рейтинг IMDB: {RateIMDB} / 10\n";

        }
    }
}
