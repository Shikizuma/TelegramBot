using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Models
{
	internal class Film
	{
        public string Name { get; set; }
		public string Description { get; set; }
		public string Genre { get; set; }
		public int Rate { get; set; }
		public int Views { get; set; }
    }
}
