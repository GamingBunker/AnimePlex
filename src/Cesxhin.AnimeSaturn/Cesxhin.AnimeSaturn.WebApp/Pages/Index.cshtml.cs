using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Cesxhin.AnimeSaturn.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        //public for page
        public List<AnimeDTO> listNameAnime = null;
        public AnimeDTO Anime { get; set; }
        public List<AnimeUrl> animeUrl = null;
        public string Error { get; set; }
        public string Success { get; set; }

        //set variable
        private readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API");
        private readonly string _port = Environment.GetEnvironmentVariable("PORT_API");
        private readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API");

        public void OnGet()
        {
            getListNameAnime();
        }

        public IActionResult OnPost()
        {
            string name = Request.Form["name"];
            string urlPage = Request.Form["url_anime_radio"];
            string searchInternal = Request.Form["btnSearchInternal"];
            string searchExternal = Request.Form["btnSearchExternal"];

            //get info by my db
            if (searchInternal == "true")
            {
                if (name != null && name.Length > 0)
                {
                    try
                    {
                        return RedirectToPage("ViewAnime", new {nameAnime = name});
                        //list name animes
                    }
                    catch
                    {
                        Error = "Not found on my db, try search on AnimeSaturn";
                    }
                }
            }else if(searchExternal == "true")
            {
                animeUrl = HtmlAnimeSaturn.GetAnimeUrl(name);
            //insert info into db
            }else if (urlPage != null)
            {
                var anime = HtmlAnimeSaturn.GetAnime(urlPage);
                var episodes = HtmlAnimeSaturn.GetEpisodes(urlPage, anime.Name);

                HttpClient client = new HttpClient();
                //anime
                using (var content = new StringContent(JsonSerializer.Serialize(anime), System.Text.Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage result = client.PostAsync($"{_protocol}://{_address}:{_port}/anime", content).Result;
                    if (result.StatusCode == HttpStatusCode.Created)
                        Success = "Inserito con successo! Adesso in automatico partiranno i download dell'anime!";
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                        Error = "Questo anime esiste già nel database, ricontrolla di nuovo per favore!";
                }

                using (var content = new StringContent(JsonSerializer.Serialize(episodes), System.Text.Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage result = client.PostAsync($"{_protocol}://{_address}:{_port}/episodes", content).Result;
                    if (result.StatusCode == HttpStatusCode.Created)
                        Success += "Ok i download saranno iniziati!";
                    else if (result.StatusCode == HttpStatusCode.Conflict)
                        Error += "Questo anime esiste già nel database, ricontrolla di nuovo per favore!";
                }
            }
            getListNameAnime();
            return Page();
        }

        private void getListNameAnime()
        {
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync($"{_protocol}://{_address}:{_port}/anime/").GetAwaiter().GetResult();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            listNameAnime = JsonSerializer.Deserialize<List<AnimeDTO>>(result, options);
        }
    }
}
