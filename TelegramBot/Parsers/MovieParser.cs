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
using Newtonsoft.Json;

namespace TelegramBot.Parsers
{
    internal class MovieParser
    {
        private const string BaseUrl = "https://uakino.club/filmy/page/1/";

        private void SaveMoviesToJson(List<FilmModel> movies, string jsonFilePath)
        {
            using (StreamWriter writer = new StreamWriter(jsonFilePath))
            {
                string json = JsonConvert.SerializeObject(movies, Formatting.Indented);
                writer.Write(json);
            }
        }

        public void ParseMovies()
        {
            var web = new HtmlWeb();
            var doc = web.Load(BaseUrl);
            var movies = new List<FilmModel>();

            var movieItems = doc.DocumentNode.SelectNodes("//div[contains(@class, 'movie-item')]");

            if (movieItems != null)
            {
                foreach (var movieItem in movieItems)
                {
                    var movie = ParseMovie(movieItem);
                    if (movie != null)
                    {
                        movies.Add(movie);
                    }
                }
            }

            SaveMoviesToJson(movies, "movies.json");
        }

        private FilmModel ParseMovie(HtmlNode movieItem)
        {
            var movie = new FilmModel();
            var movieNode = movieItem.SelectSingleNode(".//a[@class='movie-title']");

            HtmlNode descriptionNode = movieItem.SelectSingleNode(".//span[@class='desc-about-text']");
            if (descriptionNode != null)
            {
                movie.Description = descriptionNode.InnerText.Trim();
            }

            if (movieNode != null)
            {
                movie.Name = movieNode.InnerText.Trim();
                movie.MovieUrl = movieNode.GetAttributeValue("href", "");

                if (!string.IsNullOrWhiteSpace(movie.MovieUrl))
                {
                    var webInfo = new HtmlWeb();
                    var filmInfoDoc = webInfo.Load(movie.MovieUrl);

                    ParseFilmInfo(movie, filmInfoDoc);
                }
            }

            return movie;
        }

        private void ParseFilmInfo(FilmModel movie, HtmlDocument filmInfoDoc)
        {
            var linkNode = filmInfoDoc.DocumentNode.SelectSingleNode("//a[@data-fancybox='gallery']");
            if (linkNode != null)
            {
                const string BaseUrl = "https://uakino.club";
                var relativeUrl = linkNode.GetAttributeValue("href", "");
                movie.Image = BaseUrl + relativeUrl;
            }

            var infoNodes = filmInfoDoc.DocumentNode.SelectNodes("//div[contains(@class, 'fi-item')]");
            if (infoNodes != null)
            {
                foreach (var infoNode in infoNodes)
                {
                    var labelNode = infoNode.SelectSingleNode(".//div[@class='fi-label']/h2");
                    var descNode = infoNode.SelectSingleNode(".//div[@class='fi-desc']");

                    if (descNode != null)
                    {
                        string labelText = "";
                        if (labelNode != null)
                            labelText = labelNode.InnerText.Trim();
                        string descText = descNode.InnerText.Trim();

                        switch (labelText)
                        {
                            case "Рік виходу:":
                                if (int.TryParse(descText, out int release))
                                {
                                    movie.ReleaseYear = release;
                                }
                                break;

                            case "Країна:":
                                var countryNodes = descNode.SelectNodes(".//a");
                                if (countryNodes != null)
                                {
                                    foreach (var countryNode in countryNodes)
                                    {
                                        var country = countryNode.InnerText.Trim();
                                        movie.Countries.Add(country);
                                    }
                                }
                                break;

                            case "Жанр:":
                                var genreNodes = descNode.SelectNodes(".//a");
                                if (genreNodes != null)
                                {
                                    foreach (var genreNode in genreNodes)
                                    {
                                        var genre = genreNode.InnerText.Trim();
                                        movie.Genres.Add(genre);
                                    }
                                }
                                break;

                            case "Режисер:":
                                var producerNodes = descNode.SelectNodes(".//a");
                                if (producerNodes != null)
                                {
                                    foreach (var producerNode in producerNodes)
                                    {
                                        var producerUrl = producerNode.GetAttributeValue("href", "");
                                        var producer = producerNode.InnerText.Trim();
                                        movie.Director.Add(producerUrl, producer);
                                    }
                                }
                                break;

                            case "Актори:":
                                var actorsNodes = descNode.SelectNodes(".//a");
                                if (actorsNodes != null)
                                {
                                    foreach (var actorNode in actorsNodes)
                                    {
                                        var actorUrl = actorNode.GetAttributeValue("href", "");
                                        var actor = actorNode.InnerText.Trim();
                                        movie.Actors.Add(actorUrl, actor);
                                    }
                                }
                                break;

                            case "Тривалість:":
                                movie.Duration = descText;
                                break;

                            default:
                                var rateNode = infoNode.SelectSingleNode(".//div[@class='fi-label']/img[contains(@alt, 'imdb рейтинг')]");
                                if (rateNode != null)
                                {
                                    if (descNode != null)
                                    {
                                        string ratingText = descNode.InnerText.Trim();

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
                                break;
                        }
                    }
                }
            }
        }
    }
}