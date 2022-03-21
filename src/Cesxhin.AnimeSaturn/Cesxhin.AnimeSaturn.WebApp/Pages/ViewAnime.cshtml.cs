using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Cesxhin.AnimeSaturn.WebApp.Pages
{
    public class ViewAnimeModel : PageModel
    {
        [BindProperty(Name = "nameAnime", SupportsGet = true)]
        public string NameAnime { get; set; }

        public AnimeDTO Anime = null;
        public string Error { get; set; }


        //set variable
        private readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API");
        private readonly string _port = Environment.GetEnvironmentVariable("PORT_API");
        private readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API");

        public void OnGet()
        {
            HttpClient client = new HttpClient();
            string result = null;
            try
            {
                result = client.GetStringAsync($"{_protocol}://{_address}:{_port}/anime/name/" + NameAnime).GetAwaiter().GetResult();
            }catch(HttpRequestException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        Error = "Anime non trovato!";
                else if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        Error = "Errore Generico";

            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            try
            {
                Anime = JsonSerializer.Deserialize<AnimeDTO>(result, options);
            }
            catch 
            {
                Error = "Anime non trovato";
            }
        }
    }
}
