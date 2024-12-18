﻿using Microsoft.Office.Interop.Excel;
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
using Telegram.Bot.Requests;
using System.Net.Http.Json;

namespace TelegramBot.Parsers
{
    internal static class MovieParser
    {
        private const string BaseUrl = "https://uakino.club/filmy/page/1/";

        private static List<FilmModel> SaveMoviesToJson(List<FilmModel> movies, string jsonFilePath)
        {
            List<FilmModel> existingMovies = new List<FilmModel>();

            if (File.Exists(jsonFilePath))
            {
                using (StreamReader reader = new StreamReader(jsonFilePath))
                {
                    string jsonContent = reader.ReadToEnd();
                    existingMovies = JsonConvert.DeserializeObject<List<FilmModel>>(jsonContent) ?? new List<FilmModel>();
                }
            }

            foreach (var movie in movies)
            {
                if (!existingMovies.Any(existingMovie => existingMovie.Id == movie.Id))
                {
                    existingMovies.Add(movie);
                }
            }

            GetGenresToJson(existingMovies);

            using (StreamWriter writer = new StreamWriter(jsonFilePath))
            {
                string json = JsonConvert.SerializeObject(existingMovies, Formatting.Indented);
                writer.Write(json);
            }

            return existingMovies;
        }

        public static List<FilmModel> ParseMovies()
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

            movies = SaveMoviesToJson(movies, "movies.json");

            return movies;
        }

        private static FilmModel ParseMovie(HtmlNode movieItem)
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

        private static void ParseFilmInfo(FilmModel movie, HtmlDocument filmInfoDoc)
        {
            var imgElement = filmInfoDoc.DocumentNode.SelectSingleNode(".//div[@class='film-poster']//a");

            if (imgElement != null)
            {
                const string BaseUrl = "https://uakino.club";
                var relativeUrl = imgElement.GetAttributeValue("href", "");
                if (!relativeUrl.Contains(BaseUrl))
                    movie.Image = BaseUrl + relativeUrl;
                else 
                    movie.Image =  relativeUrl;
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
                                ParseNodesAndAddToCollection(descNode, movie.Countries);
                                break;

                            case "Жанр:":
                                ParseNodesAndAddToCollection(descNode, movie.Genres);             
                                break;

                            case "Режисер:":
                                ParseNodesAndAddToCollection(descNode, movie.Directors);
                                break;

                            case "Актори:":
                                ParseNodesAndAddToCollection(descNode, movie.Actors);
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
                                            movie.RateIMDB = double.Parse(imdbRating, culture);
                                            movie.ViewsIMDB = int.Parse(numberOfVotes.Replace(" ", ""));
                                        }
                                    }
                                }
                                break;
                        }

                      
                    }
                }
                movie.Id = GenerateID(movie.Name, movie.ReleaseYear, movie.Directors.FirstOrDefault().Value);
            }
        }

        private static void ParseNodesAndAddToCollection(HtmlNode descNode, List<string> collection)
        {
            var nodes = descNode.SelectNodes(".//a");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var value = node.InnerText.Trim();
                    collection.Add(value);
                }
            }
        }

        private static int GenerateID(string name, int year, string director)
        {
            
            int id = year;
            foreach(char c in name)
            {
                id += c;
            }
            if (director != null)
            {
                foreach (char c in director)
                {
                    id += c;
                }
            }
       
            return id;
        }

        private static void ParseNodesAndAddToCollection(HtmlNode descNode, Dictionary<string, string> collection)
        {
            var nodes = descNode.SelectNodes(".//a");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var url = node.GetAttributeValue("href", "");
                    var value = node.InnerText.Trim();
                    if(collection.ContainsKey(url))
                        collection.Add(url, value);
                }
            }
        }

        private static void GetGenresToJson(List<FilmModel> movies)
        {
            string jsonFilePath = "configMenu.json";
            Dictionary<string, string[][]> menuData = new();

            using (StreamReader reader = new StreamReader(jsonFilePath))
            {
                string jsonContent = reader.ReadToEnd();
                menuData = JsonConvert.DeserializeObject<Dictionary<string, string[][]>>(jsonContent);
            }


            List<string> genres = movies
                .SelectMany(movie => movie.Genres)
                .Distinct()
                .ToList();

            List<string[]> genresArrays = new List<string[]>();
            while (genres.Count > 0)
            {
                int chunkSize = Math.Min(4, genres.Count);
                string[] genreArray = new string[chunkSize];
                for (int j = 0; j < chunkSize; j++)
                {
                    genreArray[j] = genres[0];
                    genres.RemoveAt(0);
                }
                genresArrays.Add(genreArray);
            }

            genresArrays.Add(new string[] { "Назад у меню" });

            menuData["GenreMenu"] = genresArrays.ToArray();



            using (StreamWriter writer = new StreamWriter(jsonFilePath))
            {
                string json = JsonConvert.SerializeObject(menuData, Formatting.Indented);
                writer.Write(json);
            }
        }

    }
}