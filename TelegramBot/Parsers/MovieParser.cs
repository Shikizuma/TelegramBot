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

            const string URL = "https://uakino.club/page/2/";

            var web = new HtmlWeb();
            var doc = web.Load(URL);
            List<FilmModel> movies = new List<FilmModel>();

            HtmlNodeCollection movieItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'movie-item')]");

            if (movieItems != null)
            {
                foreach (HtmlNode movieItem in movieItems)
                {
                    FilmModel movie = new FilmModel();
                    HtmlNode movieNodes = movieItem.SelectSingleNode(".//a[@class='movie-title']");

                    if (movieNodes != null)
                    {
                        movie.Name = movieNodes.InnerText;
                        movie.MovieUrl = movieNodes.GetAttributeValue("href", "");

                        if (!string.IsNullOrWhiteSpace(movie.MovieUrl))
                        {
                            var webInfo = new HtmlWeb();
                            var filmInfoDoc = webInfo.Load(movie.MovieUrl);

                            HtmlNodeCollection infoNodes = filmInfoDoc.DocumentNode.SelectNodes("//div[contains(@class, 'fi-item')]");

                            foreach (HtmlNode infoNode in infoNodes)
                            {
                                // Запарсувати страну
                                HtmlNode countryNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-desc']/a");
                                string country = countryNode.InnerText;

                                // Запарсувати жанр
                                var genreNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-item-s clearfix'][1]/div[@class='fi-desc']");
                                string genre = genreNode.InnerText.Trim();

                                // Запарсувати режисера
                                var directorNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-item-s clearfix'][2]/div[@class='fi-desc']");
                                string director = directorNode.InnerText.Trim();

                                // Запарсувати акторів
                                var actorsNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-item-s clearfix'][3]/div[@class='fi-desc']");
                                string actors = actorsNode.InnerText.Trim();

                                // Запарсувати рейтинг
                                var ratingNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-item-s clearfix'][4]/div[@class='fi-desc']");
                                string rating = ratingNode.InnerText.Trim();
                                // Extract the IMDb rating

                                HtmlNode imdbRatingNode = filmInfoDoc.DocumentNode.SelectSingleNode("//div[@class='fi-item-s']/div[@class='fi-label']/img[@alt='Лікарка Ча imdb рейтинг']/../following-sibling::div[@class='fi-desc']");
                                string imdbRating = imdbRatingNode?.InnerText.Trim();

                                if (!string.IsNullOrEmpty(imdbRating))
                                {
                                    string[] parts = imdbRating.Split('/');
                                    if (parts.Length >= 2)
                                    {
                                        movie.Rate = double.Parse(parts[0].Trim());
                                        movie.Views = int.Parse(parts[1].Trim());
                                    }
                                }
                            }

                        }

                        movies.Add(movie);
                    }

                }
            }


           
        }

    }
    
}
