using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TelegramBot.Models;
using System.Globalization;

namespace TelegramBot.Parsers
{
    internal class MovieParser
    {
        private const string URL = "https://uakino.club/filmy/page/1/";

        public List<FilmModel> ParseMovies()
        {
            var web = new HtmlWeb();
            var doc = web.Load(URL);
            List<FilmModel> movies = new List<FilmModel>();

            HtmlNodeCollection movieItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'movie-item')]");

            if (movieItems != null)
            {
                foreach (HtmlNode movieItem in movieItems)
                {
                    FilmModel movie = new FilmModel();
                    HtmlNode movieNode = movieItem.SelectSingleNode(".//a[@class='movie-title']");

                    // Опис фільму
                    HtmlNode descriptionNode = movieItem.SelectSingleNode("//span[@class='desc-about-text']");
                    if (descriptionNode != null)
                    {
                        movie.Description = descriptionNode.InnerText.Trim();
                    }

                    if (movieNode != null)
                    {
                        // Назва Фільму
                        movie.Name = movieNode.InnerText.Trim();

                        // Ланка на фільм
                        movie.MovieUrl = movieNode.GetAttributeValue("href", "");

                        if (!string.IsNullOrWhiteSpace(movie.MovieUrl))
                        {
                            var webInfo = new HtmlWeb();
                            var filmInfoDoc = webInfo.Load(movie.MovieUrl);

                            // Зображення 
                            HtmlNode linkNode = filmInfoDoc.DocumentNode.SelectSingleNode("//a[@data-fancybox='gallery']");
                            if (linkNode != null)
                            {
                                const string BASEURL = "https://uakino.club";
                                string relativeUrl = linkNode.GetAttributeValue("href", "");
                                movie.Image = BASEURL + relativeUrl;
                            }

                            // Детальна інформація
                            HtmlNodeCollection infoNodes = filmInfoDoc.DocumentNode.SelectNodes("//div[contains(@class, 'fi-item')]");
                            foreach (HtmlNode infoNode in infoNodes)
                            {
                                var labelNode = infoNode.SelectSingleNode(".//div[@class='fi-label']/h2");

                                // Рік випуску
                                if (labelNode != null && labelNode.InnerText.Trim() == "Рік виходу:")
                                {
                                    HtmlNode releaseNode = infoNode.SelectSingleNode(".//div[@class='fi-desc']/a");
                                    if (releaseNode != null && int.TryParse(releaseNode.InnerText.Trim(), out int release))
                                    {
                                        movie.ReleaseYear = release;
                                        continue;
                                    }
                                }

                                // Країна
                                if (labelNode != null && labelNode.InnerText.Trim() == "Країна:")
                                {
                                    HtmlNodeCollection countryNodes = infoNode.SelectNodes(".//div[@class='fi-desc']/a");
                                    if (countryNodes != null)
                                    {
                                        foreach (HtmlNode countryNode in countryNodes)
                                        {
                                            string country = countryNode.InnerText.Trim();
                                            movie.Countries.Add(country);
                                        }
                                        continue;
                                    }
                                }

                                // Жанр
                                if (labelNode != null && labelNode.InnerText.Trim() == "Жанр:")
                                {
                                    HtmlNodeCollection genreNodes = infoNode.SelectNodes(".//div[@class='fi-desc']/a");
                                    if (genreNodes != null)
                                    {
                                        foreach (HtmlNode genreNode in genreNodes)
                                        {
                                            string genre = genreNode.InnerText.Trim();
                                            movie.Genres.Add(genre);

                                        }
                                        continue;
                                    }
                                }

                                // Режисер
                                if (labelNode != null && labelNode.InnerText.Trim() == "Режисер:")
                                {
                                    HtmlNodeCollection producerNodes = infoNode.SelectNodes(".//div[@class='fi-desc']/a");
                                    if (producerNodes != null)
                                    {
                                        foreach (HtmlNode producerNode in producerNodes)
                                        {
                                            string producerUrl = producerNode.GetAttributeValue("href", "");
                                            string producer = producerNode.InnerText.Trim();
                                            movie.Director.Add(producerUrl, producer);
                                        }
                                        continue;
                                    }
                                }

                                // Актори
                                if (labelNode != null && labelNode.InnerText.Trim() == "Актори:")
                                {
                                    HtmlNodeCollection actorsNodes = infoNode.SelectNodes(".//div[@class='fi-desc']/a");
                                    if (actorsNodes != null)
                                    {
                                        foreach (HtmlNode actorNode in actorsNodes)
                                        {
                                            string actorUrl = actorNode.GetAttributeValue("href", "");
                                            string actor = actorNode.InnerText.Trim();
                                            movie.Actors.Add(actorUrl, actor);
                                        }
                                        continue;
                                    }
                                }

                                // Тривалість
                                if (labelNode != null && labelNode.InnerText.Trim() == "Тривалість:")
                                {
                                    HtmlNode durationNode = infoNode.SelectSingleNode(".//div[@class='fi-desc']");
                                    if (durationNode != null)
                                    {
                                        movie.Duration = durationNode.InnerText.Trim();
                                    }
                                    continue;
                                }

                                // Рейтинг
                                var imgNode = infoNode.SelectSingleNode(".//div[@class='fi-label']/img[contains(@alt, 'imdb рейтинг')]");
                                if (imgNode != null)
                                {
                                    HtmlNode ratingNode = infoNode.SelectSingleNode(".//div[@class='fi-desc']");
                                    if (ratingNode != null)
                                    {
                                        string ratingText = ratingNode.InnerText.Trim();

                                        string[] ratingParts = ratingText.Split('/');
                                        if (ratingParts.Length == 2)
                                        {
                                            string imdbRating = ratingParts[0].Trim();
                                            string numberOfVotes = ratingParts[1].Trim();


                                            CultureInfo culture = CultureInfo.InvariantCulture;
                                            movie.Rate = double.Parse(imdbRating, culture);
                                            movie.Views = int.Parse(numberOfVotes);
                                        }
                                    }
                                }
                            }

                        }

                        movies.Add(movie);
                    }

                }
            }
            return movies;
        }
        

    }
    
}
