using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
	internal class FilmModel
	{
        public string Name { get; set; }
		public string Description { get; set; }
		public string Genre { get; set; }
		public double Rate { get; set; }
		public double Views { get; set; }
		public string Image { get; set; }
	}
}
