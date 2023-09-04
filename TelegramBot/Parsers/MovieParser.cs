using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TelegramBot.Models;

namespace TelegramBot.Parsers
{
    internal class MovieParser
    {
        private const string URL = "https://uakino.club/page/1/";

        public List<FilmModel> ParseMovies()
        {
            var web = new HtmlWeb();
            var doc = web.Load(URL);

            HtmlNodeCollection movieItems = doc.DocumentNode.SelectNodes("//div[@class='movie-item']");
            List<FilmModel> movies = new List<FilmModel>();
            
            foreach (HtmlNode movieItem in movieItems)
            {
                FilmModel movie = new FilmModel();
                HtmlNode movieNode = movieItem.SelectSingleNode("//a[@class='movie-title']");
                movie.Name = movieNode.InnerText;
                movies.Add(movie);
            }

        }

        private bool IsValidUrl(string url)
        {
            Regex regex = new Regex(@"https?://\S+", RegexOptions.IgnoreCase);
            return regex.IsMatch(url);
        }

    }
}
