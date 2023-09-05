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
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public List<string> Countries { get; set; }
        public Dictionary<string, string> Director { get; set; }
        public Dictionary<string, string> Actors { get; set; }
        public List<string> Genres { get; set; }
        public string Duration { get; set; }
        public double Rate { get; set; }
        public int Views { get; set; }
        public string Image { get; set; }
        public string MovieUrl { get; set; }

        public FilmModel()
        {
            Countries = new List<string>();
            Director = new Dictionary<string, string>();
            Actors = new Dictionary<string, string>();
            Genres = new List<string>();
        }
    }
}
